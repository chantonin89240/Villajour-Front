using VillajourFrontend.Entity;

namespace VillajourFrontend.Dto.Event;

public class EventByMairieFavoriteDto
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Address { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DocumentType? EventType { get; set; }
    public Mairie? Mairie { get; set; }
    public bool Favorite { get; set; }
}
