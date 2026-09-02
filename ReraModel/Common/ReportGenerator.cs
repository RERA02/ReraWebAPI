namespace ReraModel.Common
{
    public class ConnectionConfig
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string name { get; set; } = string.Empty;
        public string type { get; set; } = "sqlserver"; // sqlserver | mysql | postgresql
        public string host { get; set; } = string.Empty;
        public int port { get; set; }
        public string database { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string? connectionString { get; set; }
    }

    public class QueryRequest
    {
        public string sql { get; set; } = string.Empty;
        public string connectionId { get; set; } = string.Empty;
        public Dictionary<string, object>? parameters { get; set; }
    }

    public class QueryResult
    {
        public List<ResultSet> resultSets { get; set; } = new();
        public long elapsedMs { get; set; }
        public string? error { get; set; }
        public int affectedRows { get; set; }
    }
    public class ResultSet
    {
        public List<string> columns { get; set; } = new();
        public List<List<object?>> rows { get; set; } = new();
        public int rowCount { get; set; }
    }

    public class SavedQuery
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string name { get; set; } = string.Empty;
        public string sql { get; set; } = string.Empty;
        public string? connectionId { get; set; }
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        public DateTime updatedAt { get; set; } = DateTime.UtcNow;
    }

    public class TableInfo
    {
        public string schema { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string type { get; set; } = "TABLE"; // TABLE | VIEW
        public List<ColumnInfo> columns { get; set; } = new();
    }

    public class ColumnInfo
    {
        public string name { get; set; } = string.Empty;
        public string dataType { get; set; } = string.Empty;
        public bool isNullable { get; set; }
        public bool isPrimaryKey { get; set; }
        public string? defaultValue { get; set; }
    }

    public class StoredProcedureInfo
    {
        public string schema { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
    }

    public class FunctionInfo
    {
        public string schema { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string returnType { get; set; } = string.Empty;
    }

    public class UserDefinedTypeInfo
    {
        public string schema { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string baseType { get; set; } = string.Empty;
    }

    public class SchemaResult
    {
        public List<TableInfo> tables { get; set; } = new();
        public List<TableInfo> views { get; set; } = new();
        public List<StoredProcedureInfo> storedProcedures { get; set; } = new();
        public List<FunctionInfo> functions { get; set; } = new();
        public List<UserDefinedTypeInfo> userDefinedTypes { get; set; } = new();
        public string? error { get; set; }
    }

    public class ObjectDefinitionResult
    {
        public string definition { get; set; } = string.Empty;
    }

    public class TestConnectionRequest
    {
        public ConnectionConfig connection { get; set; } = new();
    }

    public class TestConnectionResult
    {
        public bool success { get; set; }
        public string? message { get; set; }
        public string? serverVersion { get; set; }
    }

    public class AppUser
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string username { get; set; } = string.Empty;
        public string passwordHash { get; set; } = string.Empty;
        public string role { get; set; } = "user"; // admin | user
        public bool isActive { get; set; } = true;
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
    }

    public class LoginRequest
    {
        public string? username { get; set; }
        public string? password { get; set; }
        public string? apiKey { get; set; }
    }

    public class LoginResponse
    {
        public string token { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
        public DateTime expiresAt { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string currentPassword { get; set; } = string.Empty;
        public string newPassword { get; set; } = string.Empty;
    }
}
