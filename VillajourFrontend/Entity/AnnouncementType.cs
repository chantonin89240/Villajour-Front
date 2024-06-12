using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VillajourFrontend.Entity
{
    public class AnnouncementType
    {
        [Key]
        [Required]
        [Column(Order = 0)]
        public int Id { get; set; }

        [Required]
        [Column(Order = 1)]
        public string? Libelle { get; set; }
    }
}
