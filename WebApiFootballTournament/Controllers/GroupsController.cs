using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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

            var shapedGroups = _mapper.Map<IEnumerable<GroupDto>>(groupsFromRepo)
                   .ShapeData(groupsResourceParameters.Fields);

            var shapedAuthorsWithLinks = shapedGroups.Select(author =>
            {
                var groupAsDictionary = author as IDictionary<string, object>;
                var groupLinks = CreateLinksForGroup((Guid)groupAsDictionary["Id"], null);
                groupAsDictionary.Add("links", groupLinks);
                return groupAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedAuthorsWithLinks
            };

            return Ok(linkedCollectionResource);

        }

        /// <summary>
        /// Get a list of teams for group
        /// </summary>
        /// <param name="groupId">The id of the team you want to get</param>
        /// <param name="groupsResourceParameters">The property fields of the teams you want to get</param>
        /// <returns>An ActionResult of type IEnumerable of Team for Group</returns>
        [HttpGet("{groupId}/teams", Name = "GetGroups")]
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

            var teamsForGroupFromRepo = _footballTournamentRepository.GetTeamsForGroup(groupId);

            var shapedTeams = _mapper.Map<IEnumerable<TeamDto>>(teamsForGroupFromRepo)
                   .ShapeData(groupsResourceParameters.Fields);

            var shapedTeamsWithLinks = shapedTeams.Select(author =>
            {
                var teamAsDictionary = author as IDictionary<string, object>;
                var teamLinks = CreateLinksForTeam((Guid)teamAsDictionary["Id"], null);
                teamAsDictionary.Add("links", teamLinks);
                return teamAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedTeamsWithLinks
            };

            return Ok(linkedCollectionResource);
            // return Ok(_mapper.Map<IEnumerable<TeamDto>>(coursesForAuthorFromRepo).ShapeData(groupsResourceParameters.Fields));
        }

        /// <summary>
        /// Get a group
        /// </summary>
        /// <param name="groupId">The id of the team you want to get</param>
        /// <param name="fields">The property fields of the team you want to get</param>
        /// <param name="mediaType">The media type to display links or not</param>
        /// <returns>An ActionResult of type Group</returns>
        [Produces("application/xml",
            "application/json",
            "application/vnd.marvin.hateoas+json")]
        [HttpGet("{groupId}", Name = "GetGroup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetGroup(Guid groupId, string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType,
                out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<TeamDto>(fields))
            {
                return BadRequest();
            }

            if (!_footballTournamentRepository.GroupExists(groupId))
            {
                return NotFound();
            }

            var groupFromRepo = _footballTournamentRepository.GetGroup(groupId);

            if (parsedMediaType.MediaType == "application/vnd.marvin.hateoas+json")
            {
                var links = CreateLinksForGroup(groupFromRepo.Id, null);

                var linkedResourceToReturn = groupFromRepo.ShapeData(null)
                    as IDictionary<string, object>;
                linkedResourceToReturn.Add("links", links);

                return Ok(linkedResourceToReturn);
            }

            return Ok(_mapper.Map<GroupDto>(groupFromRepo).ShapeData(fields));
        }

        /// <summary>
        /// Create a group
        /// </summary>
        /// <param name="group">The group</param>
        /// <returns>An ActionResult of type Group</returns>
        [HttpPost(Name = "CreateGroup")]
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

        private string CreateGroupsResourceUri(GroupsResourceParameters groupsResourceParameters)
        {

            return Url.Link("GetGroups",
            new
            {
                fields = groupsResourceParameters.Fields
            });

        }

        private IEnumerable<LinkDto> CreateLinksForGroup(Guid groupId, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                  new LinkDto(Url.Link("GetGroup", new { groupId }),
                  "self",
                  "GET"));
            }
            else
            {
                links.Add(
                  new LinkDto(Url.Link("GetGroup", new { groupId, fields }),
                  "self",
                  "GET"));
            }

            links.Add(
                new LinkDto(Url.Link("CreateGroup", new { groupId }),
                "create_group",
                "POST"));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForTeam(Guid teamId, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                  new LinkDto(Url.Link("GetTeam", new { teamId }),
                  "self",
                  "GET"));
            }
            else
            {
                links.Add(
                  new LinkDto(Url.Link("GetTeam", new { teamId, fields }),
                  "self",
                  "GET"));
            }

            links.Add(
               new LinkDto(Url.Link("DeleteTeam", new { teamId }),
               "delete_team",
               "DELETE"));

            links.Add(
                new LinkDto(Url.Link("CreateTeam", new { teamId }),
                "create_team",
                "POST")); ;

            links.Add(
                new LinkDto(Url.Link("UpdateTeam", new { teamId }),
                "update_team",
                "PUT"));

            return links;
        }
    }
}
