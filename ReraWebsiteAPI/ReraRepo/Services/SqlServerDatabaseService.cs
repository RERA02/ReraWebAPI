using Microsoft.Data.SqlClient;
using ReraModel.Common;
using System.Diagnostics;

namespace ReraRepo.Services;

public class SqlServerDatabaseService : IDatabaseService
{
    public string BuildConnectionString(ConnectionConfig config)
    {
        if (!string.IsNullOrWhiteSpace(config.connectionString))
            return config.connectionString;

        var builder = new SqlConnectionStringBuilder
        {
            DataSource = config.port > 0 ? $"{config.host},{config.port}" : config.host,
            InitialCatalog = config.database,
            UserID = config.username,
            Password = config.password,
            TrustServerCertificate = true,
            ConnectTimeout = 30
        };

        if (string.IsNullOrWhiteSpace(config.username))
            builder.IntegratedSecurity = true;

        return builder.ConnectionString;
    }

    public async Task<TestConnectionResult> TestConnectionAsync(string connectionString)
    {
        try
        {
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            var version = conn.ServerVersion;
            return new TestConnectionResult { success = true, message = "Connected successfully", serverVersion = version };
        }
        catch (Exception ex)
        {
            return new TestConnectionResult { success = false, message = ex.Message };
        }
    }

    public async Task<QueryResult> ExecuteQueryAsync(string sql, string connectionString, Dictionary<string, object>? parameters = null)
    {
        var result = new QueryResult();
        var sw = Stopwatch.StartNew();

        try
        {
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(sql, conn);
            cmd.CommandTimeout = 120;

            if (parameters != null)
            {
                foreach (var (key, val) in parameters)
                    cmd.Parameters.AddWithValue(key.StartsWith('@') ? key : "@" + key, val ?? DBNull.Value);
            }

            await using var reader = await cmd.ExecuteReaderAsync();

            do
            {
                if (reader.FieldCount == 0) continue;
                var rs = new ResultSet();
                for (int i = 0; i < reader.FieldCount; i++)
                    rs.columns.Add(reader.GetName(i));
                while (await reader.ReadAsync())
                {
                    var row = new List<object?>();
                    for (int i = 0; i < reader.FieldCount; i++)
                        row.Add(reader.IsDBNull(i) ? null : reader.GetValue(i));
                    rs.rows.Add(row);
                }
                rs.rowCount = rs.rows.Count;
                result.resultSets.Add(rs);
            }
            while (await reader.NextResultAsync());

            result.affectedRows = reader.RecordsAffected;
        }
        catch (Exception ex)
        {
            result.error = ex.Message;
        }
        finally
        {
            sw.Stop();
            result.elapsedMs = sw.ElapsedMilliseconds;
        }

        return result;
    }

    public async Task<string> GetObjectDefinitionAsync(string schema, string name, string objectType, string connectionString)
    {
        try
        {
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            if (objectType == "udt")
                return await GetUdtDefinitionAsync(schema, name, conn);

            var fullName = $"[{schema}].[{name}]";
            await using var cmd = new SqlCommand("SELECT OBJECT_DEFINITION(OBJECT_ID(@n))", conn);
            cmd.Parameters.AddWithValue("@n", fullName);

            var definition = await cmd.ExecuteScalarAsync() as string ?? string.Empty;

            // Convert CREATE → ALTER so it's ready to modify and re-run
            definition = objectType switch
            {
                "procedure" => ReplaceFirst(definition, "CREATE PROCEDURE", "ALTER PROCEDURE"),
                "function" => ReplaceFirst(definition, "CREATE FUNCTION", "ALTER FUNCTION"),
                "view" => ReplaceFirst(definition, "CREATE VIEW", "ALTER VIEW"),
                _ => definition
            };

            return definition;
        }
        catch (Exception ex) { return $"-- Error retrieving definition: {ex.Message}"; }
    }

    private static async Task<string> GetUdtDefinitionAsync(string schema, string name, SqlConnection conn)
    {
        // Find the type and determine if it's an alias or table type
        const string kindSql = @"
            SELECT user_type_id, is_table_type
            FROM sys.types
            WHERE SCHEMA_NAME(schema_id) = @schema AND name = @name AND is_user_defined = 1";

        int userTypeId;
        bool isTableType;

        await using (var kindCmd = new SqlCommand(kindSql, conn))
        {
            kindCmd.Parameters.AddWithValue("@schema", schema);
            kindCmd.Parameters.AddWithValue("@name", name);
            await using var r = await kindCmd.ExecuteReaderAsync();
            if (!await r.ReadAsync())
                return $"-- Type [{schema}].[{name}] not found";
            userTypeId = r.GetInt32(0);
            isTableType = r.GetBoolean(1);
        }

        if (!isTableType)
        {
            // Alias (scalar) type — reconstruct CREATE TYPE … FROM …
            const string aliasSql = @"
                SELECT
                    'CREATE TYPE [' + SCHEMA_NAME(t.schema_id) + '].[' + t.name + '] FROM [' + bt.name + ']' +
                    CASE bt.name
                        WHEN 'varchar'   THEN '(' + CASE WHEN t.max_length = -1 THEN 'MAX' ELSE CAST(t.max_length       AS VARCHAR) END + ')'
                        WHEN 'char'      THEN '(' + CAST(t.max_length AS VARCHAR) + ')'
                        WHEN 'nvarchar'  THEN '(' + CASE WHEN t.max_length = -1 THEN 'MAX' ELSE CAST(t.max_length / 2   AS VARCHAR) END + ')'
                        WHEN 'nchar'     THEN '(' + CAST(t.max_length / 2 AS VARCHAR) + ')'
                        WHEN 'varbinary' THEN '(' + CASE WHEN t.max_length = -1 THEN 'MAX' ELSE CAST(t.max_length       AS VARCHAR) END + ')'
                        WHEN 'binary'    THEN '(' + CAST(t.max_length AS VARCHAR) + ')'
                        WHEN 'decimal'   THEN '(' + CAST(t.precision  AS VARCHAR) + ', ' + CAST(t.scale AS VARCHAR) + ')'
                        WHEN 'numeric'   THEN '(' + CAST(t.precision  AS VARCHAR) + ', ' + CAST(t.scale AS VARCHAR) + ')'
                        ELSE ''
                    END +
                    CASE WHEN t.is_nullable = 1 THEN ' NULL' ELSE ' NOT NULL' END
                FROM sys.types t
                INNER JOIN sys.types bt
                    ON bt.system_type_id = t.system_type_id AND bt.user_type_id = bt.system_type_id
                WHERE t.user_type_id = @id";

            await using var aliasCmd = new SqlCommand(aliasSql, conn);
            aliasCmd.Parameters.AddWithValue("@id", userTypeId);
            return await aliasCmd.ExecuteScalarAsync() as string ?? string.Empty;
        }

        // Table type — reconstruct CREATE TYPE … AS TABLE (…)
        const string colsSql = @"
            SELECT
                c.name                                                    AS col_name,
                bt.name                                                   AS type_name,
                c.max_length,
                c.precision,
                c.scale,
                c.is_nullable,
                CASE WHEN ic.column_id IS NOT NULL THEN 1 ELSE 0 END     AS is_pk
            FROM sys.table_types tt
            INNER JOIN sys.columns c  ON c.object_id  = tt.type_table_object_id
            INNER JOIN sys.types   bt ON bt.user_type_id = c.user_type_id
            LEFT  JOIN sys.index_columns ic
                ON ic.object_id = c.object_id AND ic.column_id = c.column_id AND ic.index_id = 1
            WHERE tt.user_type_id = @id
            ORDER BY c.column_id";

        var colDefs = new List<string>();
        var pkCols = new List<string>();

        await using (var colsCmd = new SqlCommand(colsSql, conn))
        {
            colsCmd.Parameters.AddWithValue("@id", userTypeId);
            await using var cr = await colsCmd.ExecuteReaderAsync();
            while (await cr.ReadAsync())
            {
                var colName = cr.GetString(0);
                var typeName = cr.GetString(1);
                var maxLen = cr.GetInt16(2);
                var prec = cr.GetByte(3);
                var scaleVal = cr.GetByte(4);
                var nullable = cr.GetBoolean(5);
                var isPk = cr.GetInt32(6) == 1;

                var typeStr = typeName switch
                {
                    "varchar" or "char" => $"[{typeName}]({(maxLen == -1 ? "MAX" : maxLen.ToString())})",
                    "nvarchar" or "nchar" => $"[{typeName}]({(maxLen == -1 ? "MAX" : (maxLen / 2).ToString())})",
                    "varbinary" or "binary" => $"[{typeName}]({(maxLen == -1 ? "MAX" : maxLen.ToString())})",
                    "decimal" or "numeric" => $"[{typeName}]({prec}, {scaleVal})",
                    _ => $"[{typeName}]"
                };

                colDefs.Add($"    [{colName}] {typeStr} {(nullable ? "NULL" : "NOT NULL")}");
                if (isPk) pkCols.Add($"[{colName}]");
            }
        }

        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"CREATE TYPE [{schema}].[{name}] AS TABLE (");
        sb.Append(string.Join(",\n", colDefs));
        if (pkCols.Count > 0)
        {
            sb.AppendLine(",");
            sb.Append($"    PRIMARY KEY ({string.Join(", ", pkCols)})");
        }
        sb.AppendLine();
        sb.Append(')');
        return sb.ToString();
    }

    private static string ReplaceFirst(string source, string oldVal, string newVal)
    {
        var idx = source.IndexOf(oldVal, StringComparison.OrdinalIgnoreCase);
        return idx < 0 ? source : source[..idx] + newVal + source[(idx + oldVal.Length)..];
    }

    public async Task<SchemaResult> GetSchemaAsync(string connectionString)
    {
        var schema = new SchemaResult();

        try
        {
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            // Get tables
            const string tablesSql = @"
                SELECT
                    TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE
                FROM INFORMATION_SCHEMA.TABLES
                ORDER BY TABLE_SCHEMA, TABLE_NAME";

            await using (var cmd = new SqlCommand(tablesSql, conn))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var t = new TableInfo
                    {
                        schema = reader.GetString(0),
                        name = reader.GetString(1),
                        type = reader.GetString(2) == "VIEW" ? "VIEW" : "TABLE"
                    };
                    if (t.type == "VIEW") schema.views.Add(t);
                    else schema.tables.Add(t);
                }
            }

            // Get columns
            const string columnsSql = @"
                SELECT
                    c.TABLE_SCHEMA, c.TABLE_NAME, c.COLUMN_NAME, c.DATA_TYPE,
                    c.IS_NULLABLE,
                    CASE WHEN kcu.COLUMN_NAME IS NOT NULL THEN 1 ELSE 0 END AS IS_PK,
                    c.COLUMN_DEFAULT
                FROM INFORMATION_SCHEMA.COLUMNS c
                LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                    ON tc.TABLE_SCHEMA = c.TABLE_SCHEMA AND tc.TABLE_NAME = c.TABLE_NAME AND tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
                LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
                    ON kcu.CONSTRAINT_NAME = tc.CONSTRAINT_NAME AND kcu.COLUMN_NAME = c.COLUMN_NAME
                ORDER BY c.TABLE_SCHEMA, c.TABLE_NAME, c.ORDINAL_POSITION";

            await using (var cmd = new SqlCommand(columnsSql, conn))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                var colMap = new Dictionary<string, TableInfo>();
                foreach (var t in schema.tables.Concat(schema.views))
                    colMap[$"{t.schema}.{t.name}"] = t;

                while (await reader.ReadAsync())
                {
                    var key = $"{reader.GetString(0)}.{reader.GetString(1)}";
                    if (!colMap.TryGetValue(key, out var table)) continue;

                    table.columns.Add(new ColumnInfo
                    {
                        name = reader.GetString(2),
                        dataType = reader.GetString(3),
                        isNullable = reader.GetString(4) == "YES",
                        isPrimaryKey = reader.GetInt32(5) == 1,
                        defaultValue = reader.IsDBNull(6) ? null : reader.GetString(6)
                    });
                }
            }
            // Stored Procedures
            const string spSql = @"
                SELECT SPECIFIC_SCHEMA, SPECIFIC_NAME
                FROM INFORMATION_SCHEMA.ROUTINES
                WHERE ROUTINE_TYPE = 'PROCEDURE'
                ORDER BY SPECIFIC_SCHEMA, SPECIFIC_NAME";

            await using (var cmd = new SqlCommand(spSql, conn))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                    schema.storedProcedures.Add(new StoredProcedureInfo
                    {
                        schema = reader.GetString(0),
                        name = reader.GetString(1)
                    });
            }

            // Functions
            const string fnSql = @"
                SELECT SPECIFIC_SCHEMA, SPECIFIC_NAME, DATA_TYPE
                FROM INFORMATION_SCHEMA.ROUTINES
                WHERE ROUTINE_TYPE = 'FUNCTION'
                ORDER BY SPECIFIC_SCHEMA, SPECIFIC_NAME";

            await using (var cmd = new SqlCommand(fnSql, conn))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                    schema.functions.Add(new FunctionInfo
                    {
                        schema = reader.GetString(0),
                        name = reader.GetString(1),
                        returnType = reader.IsDBNull(2) ? string.Empty : reader.GetString(2)
                    });
            }

            // User-Defined Types
            const string udtSql = @"
                SELECT SCHEMA_NAME(schema_id) AS schema_name, name, TYPE_NAME(system_type_id) AS base_type
                FROM sys.types
                WHERE is_user_defined = 1
                ORDER BY schema_name, name";

            await using (var cmd = new SqlCommand(udtSql, conn))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                    schema.userDefinedTypes.Add(new UserDefinedTypeInfo
                    {
                        schema = reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
                        name = reader.GetString(1),
                        baseType = reader.IsDBNull(2) ? string.Empty : reader.GetString(2)
                    });
            }
        }
        catch { /* return partial schema on error */ }

        return schema;
    }
}
