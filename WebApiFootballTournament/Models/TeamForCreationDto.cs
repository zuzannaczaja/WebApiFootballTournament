using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WebApiFootballTournament.ValidationAttributes;

namespace WebApiFootballTournament.Models
{
    [TeamsNameMustBeDifferent(
        ErrorMessage = "Team names must be different.")]
    public class TeamForCreationDto
    {
        [Required(ErrorMessage = "You should fill out a title.")]
        [MaxLength(30, ErrorMessage = "The title shouldn't have more than 30 characters.")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "The description shouldn't have more than 500 characters.")]
        public string Description { get; set; }
        public int PointsScored { get; set; }
        public int Win { get; set; }
        public int Draw { get; set; }
        public int Lost { get; set; }
        public Guid? GroupId { get; set; }
    }
}
