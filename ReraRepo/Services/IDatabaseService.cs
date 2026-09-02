using ReraModel.Common;

namespace ReraRepo.Services;

public interface IDatabaseService
{
    Task<QueryResult> ExecuteQueryAsync(string sql, string connectionString, Dictionary<string, object>? parameters = null);
    Task<SchemaResult> GetSchemaAsync(string connectionString);
    Task<TestConnectionResult> TestConnectionAsync(string connectionString);
    Task<string> GetObjectDefinitionAsync(string schema, string name, string objectType, string connectionString);
    string BuildConnectionString(ConnectionConfig config);
}
