namespace VillajourFrontend.Dto;

public class DetailMairieDto
{
    public Guid Id { get; set; }
    public string? Phone { get; set; }
    public string? Picture { get; set; }
    public string? Siret { get; set; }
    public string? Address { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public bool Favorite { get; set; }
}
