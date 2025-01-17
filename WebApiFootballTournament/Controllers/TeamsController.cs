﻿using AutoMapper;
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

            var previousPageLink = teamsFromRepo.HasPrevious ?
            CreateTeamsResourceUri(teamsResourceParameters,
            ResourceUriType.PreviousPage) : null;

            var nextPageLink = teamsFromRepo.HasNext ?
                CreateTeamsResourceUri(teamsResourceParameters,
                ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = teamsFromRepo.TotalCount,
                pageSize = teamsFromRepo.PageSize,
                currentPage = teamsFromRepo.CurrentPage,
                totalPages = teamsFromRepo.TotalPages,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            return Ok(_mapper.Map<IEnumerable<TeamDto>>(teamsFromRepo).ShapeData(teamsResourceParameters.Fields));
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

            return Ok(_mapper.Map<TeamDto>(teamFromRepo).ShapeData(fields));
        }

        /// <summary>
        /// Create a team
        /// </summary>
        /// <param name="team">The team</param>
        /// <returns>An ActionResult of type Team</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult CreateTeam(TeamForCreationDto team)
        {
            var teamEntity = _mapper.Map<Entities.Team>(team);
            _footballTournamentRepository.AddTeam(teamEntity);
            _footballTournamentRepository.Save();

            var teamToReturn = _mapper.Map<TeamDto>(team);
            return CreatedAtRoute("GetTeam", new { teamId = teamToReturn.Id }, teamToReturn);
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
        [HttpPut("{teamId}")]
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
        [HttpDelete("{teamId}")]
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
    }
}
