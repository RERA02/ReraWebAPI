using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReraModel.Common;
using ReraRepo.Services;

namespace ReraWebPortal.Controllers.Common;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize]
public class QueryController : ControllerBase
{
    private readonly ConnectionStore _connections;
    private readonly DatabaseServiceFactory _factory;

    public QueryController(ConnectionStore connections, DatabaseServiceFactory factory)
    {
        _connections = connections;
        _factory = factory;
    }

    [HttpPost("execute")]
    public async Task<ActionResult<QueryResult>> Execute([FromBody] QueryRequest request)
    {
        var conn = _connections.GetById(request.connectionId);
        if (conn is null)
            return BadRequest(new QueryResult { error = $"Connection '{request.connectionId}' not found." });

        var svc = _factory.GetService(conn.type);
        var connStr = svc.BuildConnectionString(conn);
        var result = await svc.ExecuteQueryAsync(request.sql, connStr, request.parameters);
        return Ok(result);
    }
}
