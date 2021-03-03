using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiFootballTournament.Entities;

namespace WebApiFootballTournament.Services
{
    public interface IFootballTournamentRepository
    {
        IEnumerable<Team> GetTeams();
        IEnumerable<Group> GetGroups();
        Team GetTeam(Guid teamId);
        Group GetGroup(Char groupId);
        void AddTeam(Team team);
        void AddGroup(Group group);
        bool TeamExists(Guid teamId);
        bool TeamNameExists(String teamName);
        bool GroupExists(Char groupId);
        void UpdateTeam(Team team);
        void DeleteTeam(Team team);
        bool Save();
    }
}
