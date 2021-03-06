using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
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
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/teams")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly IFootballTournamentRepository _footballTournamentRepository;
        private readonly IMapper _mapper;

        public TeamsController(IFootballTournamentRepository footballTournamentRepository, IMapper mapper)
        {
            _footballTournamentRepository = footballTournamentRepository ??
                throw new ArgumentNullException(nameof(footballTournamentRepository));

            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet()]
        public IActionResult GetTeams()
        {
            var teamsFromRepo = _footballTournamentRepository.GetTeams();
            return Ok(_mapper.Map<IEnumerable<TeamDto>>(teamsFromRepo));
        }

        [HttpGet("{teamId}", Name = "GetTeam")]
        public IActionResult GetTeam(Guid teamId)
        {
            var teamFromRepo = _footballTournamentRepository.GetTeam(teamId);

            if (teamFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<TeamDto>(teamFromRepo));
        }

        [HttpPost]
        public IActionResult CreateTeam(TeamForCreationDto team)
        {
            var teamEntity = _mapper.Map<Entities.Team>(team);
            _footballTournamentRepository.AddTeam(teamEntity);
            _footballTournamentRepository.Save();

            var teamToReturn = _mapper.Map<TeamDto>(team);
            return CreatedAtRoute("GetTeam", new { teamId = teamToReturn.Id }, teamToReturn);
        }

        [HttpOptions]
        public IActionResult GetTeamsOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST,PUT,DELETE");
            return Ok();
        }

        [HttpPatch("{teamId}")]
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

        [HttpPut("{teamId}")]
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

        [HttpDelete("{teamId}")]
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
    }
}
