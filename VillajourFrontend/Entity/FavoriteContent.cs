using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VillajourFrontend.Entity
{
    public class FavoriteContent
    {

        public int Id { get; set; }

        public Guid UserId { get; set; }

        public int? AnnouncementId { get; set; }

        public int? EventId { get; set; }
        public int? DocumentId { get; set; }
    }
}
