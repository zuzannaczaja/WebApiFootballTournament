using Microsoft.AspNetCore.Mvc;

namespace WebApiFootballTournament.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/version")]
    [ApiController]
    public class ApiController : ControllerBase
    {

        /// <summary>
        /// Get an API version
        /// </summary>
        [HttpGet]
        public string Get()
        {
            return $"Version: {ApiVersion.Default}";
        }
    }
}
