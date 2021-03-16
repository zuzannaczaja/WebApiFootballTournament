using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiFootballTournament.Entities;
using WebApiFootballTournament.Helpers;
using WebApiFootballTournament.Models;
using WebApiFootballTournament.ResourceParameters;
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
        private readonly IPropertyCheckerService _propertyCheckerService;

        public GroupsController(IFootballTournamentRepository footballTournamentRepository, IMapper mapper, IPropertyCheckerService propertyCheckerService)
        {
            _footballTournamentRepository = footballTournamentRepository ??
                throw new ArgumentNullException(nameof(footballTournamentRepository));

            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));

            _propertyCheckerService = propertyCheckerService ??
                throw new ArgumentNullException(nameof(propertyCheckerService));
        }

        /// <summary>
        /// Get a list of groups
        /// </summary>
        /// <returns>An ActionResult of type IEnumerable of Group</returns>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetGroups([FromQuery] GroupsResourceParameters groupsResourceParameters)
        {
            if (!_propertyCheckerService.TypeHasProperties<TeamDto>(groupsResourceParameters.Fields))
            {
                return BadRequest();
            }

            var groupsFromRepo = _footballTournamentRepository.GetGroups();


            return Ok(_mapper.Map<IEnumerable<GroupDto>>(groupsFromRepo).ShapeData(groupsResourceParameters.Fields));
        }

        /// <summary>
        /// Get a list of teams for group
        /// </summary>
        /// <param name="groupId">The id of the team you want to get</param>
        /// <param name="groupsResourceParameters">The property fields of the teams you want to get</param>
        /// <returns>An ActionResult of type IEnumerable of Team for Group</returns>
        [HttpGet("{groupId}/teams")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<TeamDto>> GetTeamsForGroup(Guid groupId, [FromQuery] GroupsResourceParameters groupsResourceParameters)
        {
            if (!_propertyCheckerService.TypeHasProperties<TeamDto>(groupsResourceParameters.Fields))
            {
                return BadRequest();
            }

            if (!_footballTournamentRepository.GroupExists(groupId))
            {
                return NotFound();
            }

            var coursesForAuthorFromRepo = _footballTournamentRepository.GetTeamsForGroup(groupId);
            return Ok(_mapper.Map<IEnumerable<TeamDto>>(coursesForAuthorFromRepo).ShapeData(groupsResourceParameters.Fields));
        }

        /// <summary>
        /// Get a group
        /// </summary>
        /// <param name="groupId">The id of the team you want to get</param>
        /// <param name="fields">The property fields of the team you want to get</param>
        /// <returns>An ActionResult of type Group</returns>
        [HttpGet("{groupId}", Name = "GetGroup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetGroup(Guid groupId, string fields)
        {
            if (!_propertyCheckerService.TypeHasProperties<TeamDto>(fields))
            {
                return BadRequest();
            }

            if (!_footballTournamentRepository.GroupExists(groupId))
            {
                return NotFound();
            }

            var groupFromRepo = _footballTournamentRepository.GetGroup(groupId);

            if (groupFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<GroupDto>(groupFromRepo).ShapeData(fields));
        }

        /// <summary>
        /// Create a group
        /// </summary>
        /// <param name="group">The group</param>
        /// <returns>An ActionResult of type Group</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult CreateGroup(Group group)
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

        private string CreateTeamsResourceUri(GroupsResourceParameters groupsResourceParameters)
        {

            return Url.Link("GetTeams",
            new
            {
                fields = groupsResourceParameters.Fields
            });

        }
    }
}
