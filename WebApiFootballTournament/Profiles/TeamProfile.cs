using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace WebApiFootballTournament.Profiles
{
    public class TeamProfile : Profile
    {
        public TeamProfile()
        {
            CreateMap<Entities.Team, Models.TeamDto>();
            CreateMap<Models.TeamForCreationDto, Entities.Team>();
            CreateMap<Models.TeamForCreationDto, Models.TeamDto>();
            CreateMap<Models.TeamForUpdateDto, Entities.Team>();
        }
    }
}
