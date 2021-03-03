using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiFootballTournament.Models
{
    public class GroupDto
    {
        public Char Id { get; set; }
        public ICollection<TeamDto> Teams { get; set; } = new List<TeamDto>();
    }
}
