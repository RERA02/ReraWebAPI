using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReraModel.Common;
using ReraRepo.Services;

namespace ReraWebPortal.Controllers.Common;

[ApiController]
[Route("api/saved-queries")]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize]
public class SavedQueriesController : ControllerBase
{
    private readonly SavedQueryStore _store;

    public SavedQueriesController(SavedQueryStore store)
    {
        _store = store;
    }

    [HttpGet]
    public ActionResult<List<SavedQuery>> GetAll() => Ok(_store.GetAll());

    [HttpPost]
    public ActionResult<SavedQuery> Create([FromBody] SavedQuery query)
    {
        var created = _store.Add(query);
        return CreatedAtAction(nameof(GetAll), new { id = created.id }, created);
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] SavedQuery query)
    {
        query.id = id;
        return _store.Update(query) ? Ok(query) : NotFound();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
        => _store.Delete(id) ? NoContent() : NotFound();
}
