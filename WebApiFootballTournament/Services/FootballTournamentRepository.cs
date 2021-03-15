using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiFootballTournament.DbContexts;
using WebApiFootballTournament.Entities;
using WebApiFootballTournament.Helpers;
using WebApiFootballTournament.ResourceParameters;

namespace WebApiFootballTournament.Services
{
    public class FootballTournamentRepository : IFootballTournamentRepository
    {
        private readonly FootballTournamentContext _context;

        public FootballTournamentRepository(FootballTournamentContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public FootballTournamentRepository()
        {
        }

        public IEnumerable<Team> GetTeams()
        {
            return _context.Teams.ToList<Team>();
        }

        public PagedList<Team> GetTeams(TeamsResourceParameters teamsResourceParameters)
        {
            if (teamsResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(teamsResourceParameters));
            }

            var collection = _context.Teams as IQueryable<Team>;

            if (!string.IsNullOrWhiteSpace(teamsResourceParameters.OrderBy))
            {
                collection = collection.ApplySort(teamsResourceParameters.OrderBy);
            }

            return PagedList<Team>.Create(collection, teamsResourceParameters.PageNumber, teamsResourceParameters.PageSize);
        }

        public IEnumerable<Team> GetTeamsForGroup(Guid groupId)
        {
            if (groupId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(groupId));
            }

            return _context.Teams
                        .Where(c => c.GroupId == groupId)
                        .OrderBy(c => c.PointsScored).ToList();
        }

        public IEnumerable<Group> GetGroups()
        {
            return _context.Groups.ToList<Group>();
        }

        public Team GetTeam(Guid teamId)
        {
            if (teamId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(teamId));
            }

            return _context.Teams.FirstOrDefault(t => t.Id == teamId);
        }

        public Team GetTeamForGroup(Guid teamId, Guid groupId)
        {
            if (teamId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(teamId));
            }

            if (groupId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(groupId));
            }

            return _context.Teams
              .Where(c => c.GroupId == groupId && c.Id == teamId).FirstOrDefault();
        }

        public Group GetGroup(Guid groupId)
        {
            if (groupId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(groupId));
            }

            return _context.Groups.FirstOrDefault(t => t.Id == groupId);
        }

        public void AddTeam(Team team)
        {
            if (team == null)
            {
                throw new ArgumentNullException(nameof(team));
            }

            team.Id = Guid.NewGuid();

           /* if (groupId == '\0')
            {
                throw new ArgumentNullException(nameof(groupId));

            } else
            {
                team.GroupId = groupId;
            }*/

            _context.Teams.Add(team);
        }

        public void AddGroup(Group group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            group.Id = Guid.NewGuid();

            _context.Groups.Add(group);
        }

        public bool TeamExists(Guid teamId)
        {
            if (teamId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(teamId));
            }

            return _context.Teams.Any(a => a.Id == teamId);
        }

        public bool TeamNameExists(String teamName)
        {
            if (teamName == null)
            {
                throw new ArgumentNullException(nameof(teamName));
            }

            if (_context.Teams.Any(a => a.Name == teamName))
            {
                return true;
            }

            return false;
        }

        public bool GroupExists(Guid groupId)
        {
            if (groupId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(groupId));
            }

            return _context.Groups.Any(a => a.Id == groupId);
        }

        public void UpdateTeam(Team team)
        {
            _context.Entry(team).State = EntityState.Modified;
            //update
        }

        public void UpdateGroup(Group group)
        {
            _context.Entry(group).State = EntityState.Modified;
            //update
        }

        public void DeleteTeam(Team team)
        {
            _context.Teams.Remove(team);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
