using Microsoft.AspNetCore.Components;
using Radzen;
using System.Security.Claims;
using VillajourFrontend.Dto.Announcement;
using VillajourFrontend.Entity;

namespace VillajourFrontend.Components.Pages.Mairie;

public partial class AddAnnouncementMairie : ComponentBase
{
    [Inject]
    protected HttpClient? HttpClient { get; set; }

    [Inject]
    protected DialogService? DialogService { get; set; }

    [Inject]
    protected NavigationManager? NavigationManager { get; set; }

    [Inject]
    private IHttpContextAccessor? _httpContext { get; set; }
    protected Guid MairieGuid { get => new Guid(_httpContext?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value); }

    protected List<AnnouncementType> announcementTypeList = new List<AnnouncementType>();
    protected AddAnnouncementDto newAnnouncement = new AddAnnouncementDto();

    protected override async Task OnInitializedAsync()
    {
        await LoadAnnouncementType();
    }

    protected async Task LoadAnnouncementType()
    {
        var apiUrl = "https://localhost:7205/Api/Announcement/GetAnnouncementType";
        try
        {
            var type = await HttpClient.GetFromJsonAsync<List<AnnouncementType>>(apiUrl);
            announcementTypeList = type?.ToList() ?? new List<AnnouncementType>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des annonces : {ex.Message}");
        }
    }

    protected async Task HandleValidSubmit()
    {
        var apiUrl = "Announcement";
        try
        {
            newAnnouncement.MairieId = MairieGuid;
            var response = await HttpClient.PostAsJsonAsync(apiUrl, newAnnouncement);
            if (response.IsSuccessStatusCode)
            {
                DialogService.Close(true);
            }
            else
            {
                Console.WriteLine($"Erreur lors de l'ajout de l'événement: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            // Gestion des erreurs du client
            Console.WriteLine($"Erreur lors de l'ajout de l'événement : {ex.Message}");
        }
    }
}
