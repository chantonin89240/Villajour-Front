using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VillajourFrontend.Entity
{
    public class User
    {
        public Guid Id { get; set; }
        public string? Phone { get; set; }
        public string? Picture { get; set; }
    }
}
