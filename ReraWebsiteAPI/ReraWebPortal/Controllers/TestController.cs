using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ReraWebPortal.Controllers
{
    [Route("api/analytics/[controller]")]
    [ApiController]
    [Authorize(Policy = "ApiScope")]
    [ApiExplorerSettings(IgnoreApi = true)]
    // JWT required to access this endpoint
    //[CustomeAuthorize]
    public class TestController : ControllerBase
    {
        public TestController()
        {
        }

        [HttpGet("Hello")]
        public async Task<IActionResult> GetHello()
        {
            return Ok(new { message = "Result from RERA analytics API with JWT." });
        }

        [HttpGet("Hello2")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHello2()
        {
            return Ok(new { message = "Result from RERA analytics API." });
        }
    }
}

