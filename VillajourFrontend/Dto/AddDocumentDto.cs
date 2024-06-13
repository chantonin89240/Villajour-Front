using Microsoft.AspNetCore.Components.Forms;

namespace VillajourFrontend.Dto;

public class AddDocumentDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public IBrowserFile? DocumentContent { get; set; }
    public int DocumentTypeId { get; set; }
    public Guid MairieId { get; set; }
}
