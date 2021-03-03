using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApiFootballTournament.DbContexts;
using WebApiFootballTournament.Models;
using WebApiFootballTournament.Services;

namespace WebApiFootballTournament.ValidationAttributes
{
    public class TeamsNameMustBeDifferent : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, 
            ValidationContext validationContext)
        {
            var _context = (FootballTournamentContext)validationContext
                         .GetService(typeof(FootballTournamentContext));

            var team = (TeamForCreationDto) validationContext.ObjectInstance;

            if (_context.Teams.Any(a => a.Name == team.Name)) {

                return new ValidationResult(ErrorMessage,
                    new[] { nameof(TeamForCreationDto) });
            }

            return ValidationResult.Success;
        }
    }
}
