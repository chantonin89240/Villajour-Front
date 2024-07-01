using VillajourFrontend.Entity;

namespace VillajourFrontend.Dto.Event;

public class EventByMairieDetailDto
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Address { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public EventType? EventType { get; set; }
    public bool Favorite { get; set; }
}
