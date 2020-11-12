using Microsoft.AspNetCore.Mvc;
using SW.Model;

namespace SW.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EpisodeController:ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(Episode.List);
        }
    }
}
