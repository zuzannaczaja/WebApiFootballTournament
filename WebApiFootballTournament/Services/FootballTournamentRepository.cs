using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiFootballTournament.DbContexts;
using WebApiFootballTournament.Entities;

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

        public Group GetGroup(Char groupId)
        {
            if (groupId == null)
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

            _context.Teams.Add(team);
        }

        public void AddGroup(Group group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

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

        public bool GroupExists(Char groupId)
        {
            if (groupId == null)
            {
                throw new ArgumentNullException(nameof(groupId));
            }

            return _context.Groups.Any(a => a.Id == groupId);
        }

        public void UpdateTeam(Team team)
        {
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
