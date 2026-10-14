using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReraModel.Common;
using ReraRepo.Services;

namespace ReraWebPortal.Controllers.Common;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize]
public class SchemaController : ControllerBase
{
    private readonly ConnectionStore _connections;
    private readonly DatabaseServiceFactory _factory;

    public SchemaController(ConnectionStore connections, DatabaseServiceFactory factory)
    {
        _connections = connections;
        _factory = factory;
    }

    [HttpGet("{connectionId}")]
    public async Task<ActionResult<SchemaResult>> GetSchema(string connectionId)
    {
        var conn = _connections.GetById(connectionId);
        if (conn is null)
            return NotFound(new { message = $"Connection '{connectionId}' not found." });

        var svc = _factory.GetService(conn.type);
        var connStr = svc.BuildConnectionString(conn);
        var schema = await svc.GetSchemaAsync(connStr);
        return Ok(schema);
    }
    [HttpGet("{connectionId}/definition")]
    public async Task<ActionResult<ObjectDefinitionResult>> GetDefinition(
        string connectionId,
        [FromQuery] string type,
        [FromQuery] string schema,
        [FromQuery] string name)
    {
        var conn = _connections.GetById(connectionId);
        if (conn is null)
            return NotFound(new { message = $"Connection '{connectionId}' not found." });

        var svc = _factory.GetService(conn.type);
        var connStr = svc.BuildConnectionString(conn);
        var definition = await svc.GetObjectDefinitionAsync(schema, name, type, connStr);
        return Ok(new ObjectDefinitionResult { definition = definition });
    }
}
