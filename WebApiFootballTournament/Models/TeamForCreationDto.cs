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
        [MaxLength(30, ErrorMessage = "The title shouldn't have more than 100 characters.")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "The description shouldn't have more than 1500 characters.")]
        public virtual string Description { get; set; }
    }
}
