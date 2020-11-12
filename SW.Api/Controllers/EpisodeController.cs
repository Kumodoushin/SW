using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SW.Model;
using System.Linq;

namespace SW.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EpisodeController:ControllerBase
    {
        /// <summary>
        /// Returns the list of supported Episodes
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /episode
        /// 
        /// </remarks>
        /// <returns>Ordered list of supported episodes.</returns>
        /// <response code="200">DRY</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            return Ok(Episode.List.OrderBy(x=>x.Value));
        }
    }
}
