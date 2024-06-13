namespace VillajourFrontend.Entity;

public class Document
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public byte[]? DocumentContent { get; set; }
    public int DocumentTypeId { get; set; }
    public Guid MairieId { get; set; }
}
