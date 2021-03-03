using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiFootballTournament.Models
{
    public class TeamDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int PointsScored { get; set; }

        public int Win { get; set; }

        public int Draw { get; set; }

        public int Lost { get; set; }

        public Char GroupId { get; set; }
    }
}
