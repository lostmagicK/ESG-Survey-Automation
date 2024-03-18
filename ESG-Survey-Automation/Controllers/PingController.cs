using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESG_Survey_Automation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public ActionResult Ping()
        {
            return Ok("Pong");
        }
    }
}
