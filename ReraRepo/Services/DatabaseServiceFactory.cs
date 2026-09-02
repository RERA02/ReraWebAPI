namespace ReraRepo.Services;

public class DatabaseServiceFactory
{
    private readonly SqlServerDatabaseService _sqlServer;

    public DatabaseServiceFactory(
        SqlServerDatabaseService sqlServer)
    {
        _sqlServer = sqlServer;
    }

    public IDatabaseService GetService(string dbType)
    {
        return _sqlServer;
    }
}
