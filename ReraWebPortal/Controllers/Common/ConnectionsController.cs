using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReraModel.Common;
using ReraRepo.Services;

namespace ReraWebPortal.Controllers.Common;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize]
public class ConnectionsController : ControllerBase
{
    private readonly ConnectionStore _store;
    private readonly DatabaseServiceFactory _factory;

    public ConnectionsController(ConnectionStore store, DatabaseServiceFactory factory)
    {
        _store = store;
        _factory = factory;
    }

    [HttpGet]
    public ActionResult<List<ConnectionConfig>> GetAll()
    {
        var connections = _store.GetAll().Select(c => new ConnectionConfig
        {
            id = c.id,
            name = c.name,
            type = c.type,
            host = c.host,
            port = c.port,
            database = c.database,
            username = c.username,
            password = string.IsNullOrEmpty(c.password) ? "" : "***",
            connectionString = c.connectionString
        }).ToList();
        return Ok(connections);
    }

    [HttpPost]
    public ActionResult<ConnectionConfig> Create([FromBody] ConnectionConfig config)
    {
        var created = _store.Add(config);
        return CreatedAtAction(nameof(GetAll), new { id = created.id }, created);
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] ConnectionConfig config)
    {
        config.id = id;
        return _store.Update(config) ? Ok(config) : NotFound();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
        => _store.Delete(id) ? NoContent() : NotFound();

    [HttpPost("test")]
    public async Task<ActionResult<TestConnectionResult>> Test([FromBody] TestConnectionRequest request)
    {
        var svc = _factory.GetService(request.connection.type);
        var connStr = svc.BuildConnectionString(request.connection);
        var result = await svc.TestConnectionAsync(connStr);
        return Ok(result);
    }
}
