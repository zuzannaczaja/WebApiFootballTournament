using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApiFootballTournament.ValidationAttributes;

namespace WebApiFootballTournament.Models
{
    public class TeamForUpdateDto
    {
        public int PointsScored { get; set; }
        public int Win { get; set; }
        public int Draw { get; set; }
        public int Lost { get; set; }
        public Guid? GroupId { get; set; }
    }
}