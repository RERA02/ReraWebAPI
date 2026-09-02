using ReraCore.Helper;
using ReraModel.Common;
using System.Text.Json;

namespace ReraRepo.Services;

public class ConnectionStore
{
    private readonly string _filePath;
    private List<ConnectionConfig> _connections = new();

    public ConnectionStore()
    {
        _filePath = Path.Combine(ConfigurationHelper.RootPath, "Data", "connections.json");
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        Load();
    }

    private void Load()
    {
        if (File.Exists(_filePath))
        {
            var json = File.ReadAllText(_filePath);
            _connections = JsonSerializer.Deserialize<List<ConnectionConfig>>(json) ?? new();
            if (_connections is not null && _connections.Count > 0)
                _connections[0].connectionString = ConfigurationHelper.ConnectionString;
        }
    }

    private void Save()
    {
        File.WriteAllText(_filePath, JsonSerializer.Serialize(_connections, new JsonSerializerOptions { WriteIndented = true }));
    }

    public List<ConnectionConfig> GetAll() => _connections;

    public ConnectionConfig? GetById(string id) => _connections.FirstOrDefault(c => c.id == id);

    public ConnectionConfig Add(ConnectionConfig config)
    {
        config.id = Guid.NewGuid().ToString();
        _connections.Add(config);
        Save();
        return config;
    }

    public bool Update(ConnectionConfig config)
    {
        var idx = _connections.FindIndex(c => c.id == config.id);
        if (idx < 0) return false;
        _connections[idx] = config;
        Save();
        return true;
    }

    public bool Delete(string id)
    {
        var removed = _connections.RemoveAll(c => c.id == id) > 0;
        if (removed) Save();
        return removed;
    }
}

public class SavedQueryStore
{
    private readonly string _filePath;
    private List<SavedQuery> _queries = new();

    public SavedQueryStore()
    {
        _filePath = Path.Combine(ConfigurationHelper.RootPath, "Data", "saved_queries.json");
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        Load();
    }

    private void Load()
    {
        if (File.Exists(_filePath))
        {
            var json = File.ReadAllText(_filePath);
            _queries = JsonSerializer.Deserialize<List<SavedQuery>>(json) ?? new();
        }
    }

    private void Save()
    {
        File.WriteAllText(_filePath, JsonSerializer.Serialize(_queries, new JsonSerializerOptions { WriteIndented = true }));
    }

    public List<SavedQuery> GetAll() => _queries;

    public SavedQuery? GetById(string id) => _queries.FirstOrDefault(q => q.id == id);

    public SavedQuery Add(SavedQuery query)
    {
        query.id = Guid.NewGuid().ToString();
        query.createdAt = DateTime.UtcNow;
        query.updatedAt = DateTime.UtcNow;
        _queries.Add(query);
        Save();
        return query;
    }

    public bool Update(SavedQuery query)
    {
        var idx = _queries.FindIndex(q => q.id == query.id);
        if (idx < 0) return false;
        query.updatedAt = DateTime.UtcNow;
        _queries[idx] = query;
        Save();
        return true;
    }

    public bool Delete(string id)
    {
        var removed = _queries.RemoveAll(q => q.id == id) > 0;
        if (removed) Save();
        return removed;
    }
}
