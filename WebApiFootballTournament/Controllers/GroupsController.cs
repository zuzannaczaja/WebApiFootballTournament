using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiFootballTournament.Models;
using WebApiFootballTournament.Services;

namespace WebApiFootballTournament.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/groups")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly IFootballTournamentRepository _footballTournamentRepository;
        private readonly IMapper _mapper;

        public GroupsController(IFootballTournamentRepository footballTournamentRepository, IMapper mapper)
        {
            _footballTournamentRepository = footballTournamentRepository ??
                throw new ArgumentNullException(nameof(footballTournamentRepository));

            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet()]
        public IActionResult GetGroups()
        {
            var groupsFromRepo = _footballTournamentRepository.GetGroups();
            return Ok(_mapper.Map<IEnumerable<GroupDto>>(groupsFromRepo));
        }

        [HttpGet("groupId")]
        public IActionResult GetGroup(Char groupId)
        {
            if (!_footballTournamentRepository.GroupExists(groupId))
            {
                return NotFound();
            }

            var groupFromRepo = _footballTournamentRepository.GetGroup(groupId);

            if (groupFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<GroupDto>(groupFromRepo));
        }

        [HttpPost]
        public IActionResult CreateGroup(Entities.Group group)
        {
            _footballTournamentRepository.AddGroup(group);
            _footballTournamentRepository.Save();

            return CreatedAtRoute("GetTeam", new { teamId = group.Id }, group);
        }

        [HttpOptions]
        public IActionResult GetGroupsOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }
    }
}
