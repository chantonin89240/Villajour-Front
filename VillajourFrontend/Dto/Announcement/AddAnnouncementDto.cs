﻿namespace VillajourFrontend.Dto.Announcement;

public class AddAnnouncementDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int AnnouncementTypeId { get; set; }
    public Guid MairieId { get; set; }
}
