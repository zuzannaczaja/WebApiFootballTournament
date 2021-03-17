using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiFootballTournament.Models;

namespace WebApiFootballTournament.Controllers
{
    [Route("api")]
    [ApiController]
    public class RootController : ControllerBase
    {
        /// <summary>
        /// Get an API version
        /// </summary>
        [HttpGet("{version}")]
        public string Get()
        {
            return $"Version: {ApiVersion.Default}";
        }

        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot()
        {
            // create links for root
            var links = new List<LinkDto>();

            links.Add(
              new LinkDto(Url.Link("GetRoot", new { }),
              "self",
              "GET"));

            links.Add(
              new LinkDto(Url.Link("GetTeams", new { }),
              "teams",
              "GET"));

            links.Add(
              new LinkDto(Url.Link("CreateTeam", new { }),
              "create_team",
              "POST"));

            return Ok(links);

        }
    }
}
