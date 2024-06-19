﻿namespace VillajourFrontend.Dto.Event;

public class AddEventDto
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Address { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int EventTypeId { get; set; }
    public Guid MairieId { get; set; }
}
