using Microsoft.AspNetCore.Components;
using Radzen;
using VillajourFrontend.Dto.Announcement;
using VillajourFrontend.Entity;

namespace VillajourFrontend.Components.Pages.Mairie;

public partial class UpdateAnnouncementMairie : ComponentBase
{
    [Inject]
    protected HttpClient? HttpClient { get; set; }

    [Inject]
    protected DialogService? DialogService { get; set; }

    [Inject]
    protected NavigationManager? NavigationManager { get; set; }

    [Parameter]
    public AnnouncementDto? announcement { get; set; }

    protected List<AnnouncementType> announcementTypeList = new List<AnnouncementType>();

    protected Guid MairieGuid => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

    protected override async Task OnInitializedAsync()
    {
        await LoadAnnouncementType();
    }

    protected async Task LoadAnnouncementType()
    {
        var apiUrl = "Announcement/GetAnnouncementType";
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
        var apiUrl = "Announcement/" + announcement.Id;
        try
        {
            Announcement updateAnnouncement = new Announcement()
            {
                Id = announcement.Id,
                Description = announcement.Description,
                Title = announcement.Title,
                AnnouncementTypeId = announcement.AnnouncementType.Id,
                MairieId = MairieGuid
            };

            var response = await HttpClient.PutAsJsonAsync(apiUrl, updateAnnouncement);
            if (response.IsSuccessStatusCode)
            {
                DialogService.Close(true);
            }
            else
            {
                Console.WriteLine($"Erreur lors de la modification de l'annonce : {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            // Gestion des erreurs du client
            Console.WriteLine($"Erreur lors de la modification de l'annonce : {ex.Message}");
        }
    }
}
