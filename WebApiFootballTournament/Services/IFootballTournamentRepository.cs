﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiFootballTournament.Entities;
using WebApiFootballTournament.Helpers;
using WebApiFootballTournament.ResourceParameters;

namespace WebApiFootballTournament.Services
{
    public interface IFootballTournamentRepository
    {
        PagedList<Team> GetTeams(TeamsResourceParameters teamsResourceParameters);
        IEnumerable<Team> GetTeamsForGroup(Guid groupId);
        IEnumerable<Group> GetGroups();
        Team GetTeam(Guid teamId);
        Team GetTeamForGroup(Guid teamId, Guid groupId);
        Group GetGroup(Guid groupId);
        void AddTeam(Team team);
        void AddGroup(Group group);
        bool TeamExists(Guid teamId);
        bool TeamNameExists(String teamName);
        bool GroupExists(Guid groupId);
        void UpdateTeam(Team team);
        void UpdateGroup(Group group);
        void DeleteTeam(Team team);
        bool Save();
    }
}
