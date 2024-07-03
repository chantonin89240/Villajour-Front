using VillajourFrontend.Dto.Announcement;
using VillajourFrontend.Dto.Document;
using VillajourFrontend.Dto.Event;

namespace VillajourFrontend.Dto;

public class HomeMairieDto
{
    public AnnouncementDto? Announcement { get; set; }
    public EventDto? Event { get; set; }
    public DocumentDto? Document { get; set; }
}
