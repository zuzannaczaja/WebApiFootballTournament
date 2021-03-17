using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
    [Route("api/v{version:apiVersion}/teams")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly IFootballTournamentRepository _footballTournamentRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyCheckerService _propertyCheckerService;

        public TeamsController(IFootballTournamentRepository footballTournamentRepository, IMapper mapper, IPropertyCheckerService propertyCheckerService)
        {
            _footballTournamentRepository = footballTournamentRepository ??
                throw new ArgumentNullException(nameof(footballTournamentRepository));

            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));

            _propertyCheckerService = propertyCheckerService ??
                throw new ArgumentNullException(nameof(propertyCheckerService));
        }

        /// <summary>
        /// Get a list of teams
        /// </summary>
        /// <returns>An ActionResult of type IEnumerable of Team</returns>
        [HttpGet(Name = "GetTeams")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetTeams([FromQuery] TeamsResourceParameters teamsResourceParameters)
        {

            if (!_propertyCheckerService.TypeHasProperties<TeamDto>(teamsResourceParameters.Fields))
            {
                return BadRequest();
            }

            var teamsFromRepo = _footballTournamentRepository.GetTeams(teamsResourceParameters);

            var paginationMetadata = new
            {
                totalCount = teamsFromRepo.TotalCount,
                pageSize = teamsFromRepo.PageSize,
                currentPage = teamsFromRepo.CurrentPage,
                totalPages = teamsFromRepo.TotalPages
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForTeams(teamsResourceParameters,
                teamsFromRepo.HasNext,
                teamsFromRepo.HasPrevious);

            var shapedTeams = _mapper.Map<IEnumerable<TeamDto>>(teamsFromRepo)
                   .ShapeData(teamsResourceParameters.Fields);

            var shapedTeamsWithLinks = shapedTeams.Select(author =>
            {
                var teamAsDictionary = author as IDictionary<string, object>;
                var teamLinks = CreateLinksForTeam((Guid)teamAsDictionary["Id"], null);
                teamAsDictionary.Add("links", teamLinks);
                return teamAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedTeamsWithLinks,
                links
            };

            return Ok(linkedCollectionResource);
        }

        /// <summary>
        /// Get a team
        /// </summary>
        /// <param name="teamId">The id of the team you want to get</param>
        /// <param name="fields">The property fields of the team you want to get</param>
        /// <returns>An ActionResult of type Team</returns>
        [HttpGet("{teamId}", Name = "GetTeam")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetTeam(Guid teamId, string fields)
        {
            if (!_propertyCheckerService.TypeHasProperties<TeamDto>(fields))
            {
                return BadRequest();
            }

            var teamFromRepo = _footballTournamentRepository.GetTeam(teamId);

            if (teamFromRepo == null)
            {
                return NotFound();
            }

            var links = CreateLinksForTeam(teamId, fields);

            var linkedResourceToReturn =
                _mapper.Map<TeamDto>(teamFromRepo).ShapeData(fields)
                 as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);
        }

        /// <summary>
        /// Create a team
        /// </summary>
        /// <param name="team">The team</param>
        /// <returns>An ActionResult of type Team</returns>
        [HttpPost(Name = "CreateTeam")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult CreateTeam(TeamForCreationDto team)
        {
            var teamEntity = _mapper.Map<Team>(team);
            _footballTournamentRepository.AddTeam(teamEntity);
            _footballTournamentRepository.Save();

            var teamToReturn = _mapper.Map<TeamDto>(team);

            var links = CreateLinksForTeam(teamToReturn.Id, null);

            var linkedResourceToReturn = teamToReturn.ShapeData(null)
                as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetTeam",
                new { teamId = linkedResourceToReturn["Id"] },
                linkedResourceToReturn);
        }

        /// <summary>
        /// Options
        /// </summary>
        [HttpOptions]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetTeamsOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST,PUT,DELETE");
            return Ok();
        }

        /// <summary>
        /// Partially update a team
        /// </summary>
        /// <param name="teamId">The id of the team you want to get</param>
        /// <param name="patchDocument">The set of operations to apply to the team</param>
        [HttpPatch("{teamId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult PartiallyUpdateTeam(Guid teamId,
            JsonPatchDocument<TeamForCreationDto> patchDocument)
        {
            if (!_footballTournamentRepository.TeamExists(teamId))
            {
                return NotFound();
            }

            var teamFromRepo = _footballTournamentRepository.GetTeam(teamId);
            var teamToPatch = _mapper.Map<TeamForCreationDto>(teamFromRepo);

            patchDocument.ApplyTo(teamToPatch, (Microsoft.AspNetCore.JsonPatch.Adapters.IObjectAdapter)ModelState);

            if (!TryValidateModel(teamToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(teamToPatch, teamFromRepo);

            _footballTournamentRepository.UpdateTeam(teamFromRepo);

            _footballTournamentRepository.Save();

            return NoContent();
        }

        /// <summary>
        /// Update a team
        /// </summary>
        /// <param name="teamId">The id of the team to update</param>
        /// <param name="team">The team with updated values</param>
        [HttpPut("{teamId}", Name = "UpdateTeam")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateTeam(Guid teamId, TeamForUpdateDto team)
        {
            if (!_footballTournamentRepository.TeamExists(teamId))
            {
                return NotFound();
            };

            var teamFromRepo = _footballTournamentRepository.GetTeam(teamId);

            _mapper.Map(team, teamFromRepo);

            var group = _footballTournamentRepository.GetGroup((Guid)team.GroupId);
            group.Teams.Add(teamFromRepo);

            _footballTournamentRepository.UpdateTeam(teamFromRepo);
            _footballTournamentRepository.UpdateGroup(group);

            _footballTournamentRepository.Save();
            return NoContent();
        }

        /// <summary>
        /// Delete a team
        /// </summary>
        /// <param name="teamId">The id of the team to delete</param>
        [HttpDelete("{teamId}", Name = "DeleteTeam")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteTeam(Guid teamId)
        {
            if (!_footballTournamentRepository.TeamExists(teamId))
            {
                return NotFound();
            }

            var teamFromRepo = _footballTournamentRepository.GetTeam(teamId);

            _footballTournamentRepository.DeleteTeam(teamFromRepo);
            _footballTournamentRepository.Save();

            return NoContent();
        }

        private string CreateTeamsResourceUri(TeamsResourceParameters teamsResourceParameters, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetTeams",
                      new
                      {
                          fields = teamsResourceParameters.Fields,
                          orderBy = teamsResourceParameters.OrderBy,
                          pageNumber = teamsResourceParameters.PageNumber - 1,
                          pageSize = teamsResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return Url.Link("GetTeams",
                      new
                      {
                          fields = teamsResourceParameters.Fields,
                          orderBy = teamsResourceParameters.OrderBy,
                          pageNumber = teamsResourceParameters.PageNumber + 1,
                          pageSize = teamsResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetTeams",
                    new
                    {
                        fields = teamsResourceParameters.Fields,
                        orderBy = teamsResourceParameters.OrderBy,
                        pageNumber = teamsResourceParameters.PageNumber,
                        pageSize = teamsResourceParameters.PageSize
                    });
            }

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
                "POST"));;

            links.Add(
                new LinkDto(Url.Link("UpdateTeam", new { teamId }),
                "update_team",
                "PUT"));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForTeams(
            TeamsResourceParameters teamsResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateTeamsResourceUri(
                   teamsResourceParameters, ResourceUriType.Current)
               , "self", "GET"));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateTeamsResourceUri(
                      teamsResourceParameters, ResourceUriType.NextPage),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateTeamsResourceUri(
                        teamsResourceParameters, ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }

            return links;
        }
    }
}
