namespace VillajourFrontend.Entity;

public class Announcement
{
    public int Id { get; set; } 
    public DateOnly Date { get; set; }     
    public string? Title { get; set; }       
    public string? Description { get; set; }
    public int AnnouncementTypeId { get; set; }
    public Guid MairieId { get; set; }
}
