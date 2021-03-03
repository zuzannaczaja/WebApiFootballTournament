using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiFootballTournament.Entities
{
    [Table("Teams")]
    public class Team
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        public int PointsScored { get; set; }

        public int Win { get; set; }

        public int Draw { get; set; }

        public int Lost { get; set; }

        [ForeignKey("GroupId")]
        public Char? GroupId { get; set; }
        public virtual Group Group { get; set; }
    }
}
