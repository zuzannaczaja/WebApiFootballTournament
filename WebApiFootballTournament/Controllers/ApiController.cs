using Microsoft.AspNetCore.Mvc;

namespace WebApiFootballTournament.Controllers
{
    [ApiVersion("1.0")]
    [Route("[controller]/version")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return $"Version: {ApiVersion.Default}";
        }
    }
}
