using VillajourFrontend.Entity;

namespace VillajourFrontend.Dto.Announcement;

public class AnnouncementByMairieFavoriteDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public AnnouncementType? AnnouncementType { get; set; }
    public Mairie? Mairie { get; set; }
    public bool Favorite { get; set; }
}
