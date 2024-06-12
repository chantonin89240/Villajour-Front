using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VillajourFrontend.Entity
{
    public class Announcement
    {
      
        public int Id { get; set; } 
        public DateTime Date { get; set; }     
        public string? Title { get; set; }       
        public string? Description { get; set; }
 
        public int AnnouncementTypeId { get; set; }
        public int MairieId { get; set; }
    }
}
