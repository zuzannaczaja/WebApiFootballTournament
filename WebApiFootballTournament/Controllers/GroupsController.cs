using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiFootballTournament.Entities;
using WebApiFootballTournament.Models;
using WebApiFootballTournament.Services;

namespace WebApiFootballTournament.Controllers
{
    [Produces("application/json")]
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

        /// <summary>
        /// Get a list of groups
        /// </summary>
        /// <returns>An ActionResult of type IEnumerable of Group</returns>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetGroups()
        {
            var groupsFromRepo = _footballTournamentRepository.GetGroups();
            return Ok(_mapper.Map<IEnumerable<GroupDto>>(groupsFromRepo));
        }

        /// <summary>
        /// Get a list of teams for group
        /// </summary>
        /// <param name="groupId">The id of the team you want to get</param>
        /// <returns>An ActionResult of type IEnumerable of Team for Group</returns>
        [HttpGet("{groupId}/teams")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<TeamDto>> GetTeamsForGroup(Guid groupId)
        {
            if (!_footballTournamentRepository.GroupExists(groupId))
            {
                return NotFound();
            }

            var coursesForAuthorFromRepo = _footballTournamentRepository.GetTeamsForGroup(groupId);
            return Ok(_mapper.Map<IEnumerable<TeamDto>>(coursesForAuthorFromRepo));
        }

        /// <summary>
        /// Get a group
        /// </summary>
        /// <param name="groupId">The id of the team you want to get</param>
        /// <returns>An ActionResult of type Group</returns>
        [HttpGet("{groupId}", Name = "GetGroup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetGroup(Guid groupId)
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

        /// <summary>
        /// Create a group
        /// </summary>
        /// <param name="group">The group</param>
        /// <returns>An ActionResult of type Group</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult CreateGroup(Entities.Group group)
        {
            _footballTournamentRepository.AddGroup(group);
            _footballTournamentRepository.Save();

            return CreatedAtRoute("GetGroup", new { groupId = group.Id }, group);
        }

        /// <summary>
        /// Options
        /// </summary>
        [HttpOptions]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetGroupsOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }
    }
}
