using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiFootballTournament.Entities
{
    [Table("Groups")]
    public class Group
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(1)]
        public string Name { get; set; }

        public ICollection<Team> Teams { get; set; } = new List<Team>();
    }
}
