using Microsoft.AspNetCore.Components;
using Radzen;
using VillajourFrontend.Dto.Announcement;
using VillajourFrontend.Entity;

namespace VillajourFrontend.Components.Pages.Mairie;

public partial class AnnouncementMairie 
{
    [Inject]
    protected HttpClient? HttpClient { get; set; }

    [Inject]
    protected DialogService? DialogService { get; set; }

    [Inject]
    protected NavigationManager? NavigationManager { get; set; }

    [Inject]
    private NotificationService? NotificationService { get; set; }

    protected List<AnnouncementDto> announcement = new List<AnnouncementDto>();

    protected Guid userGuid => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
    protected Guid mairieGuid => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

    protected override async Task OnInitializedAsync()
    {
        await LoadAnnouncement();
    }

    protected async Task LoadAnnouncement()
    {
        var apiUrl = $"Announcement/GetAnnouncementHistoByMairie/{mairieGuid}";
        try
        {
            var annonceMairie = await HttpClient.GetFromJsonAsync<List<AnnouncementDto>>(apiUrl);
            announcement = annonceMairie?.ToList() ?? new List<AnnouncementDto>();
            this.StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des événements : {ex.Message}");
        }
    }


    // redirige vers la page d'ajout d'une annonce
    private async void OnAddNewAnnouncement()
    {
        var newAnnouncement = new Announcement();
        var result = await DialogService.OpenAsync<AddAnnouncementMairie>("Ajouter une annonce");

        if (result != null && result)
        {
            await LoadAnnouncement();
            NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "L'annonce a été ajouté", Duration = 4000 };
            NotificationService.Notify(message);
        }
    }

    // suppression d'un event
    protected async Task OnDeleteAnnouncement(int id)
    {
        var apiUrl = $"Announcement/{id}";
        try
        {
            var validation = await HttpClient.DeleteAsync(apiUrl);

            if (validation.IsSuccessStatusCode)
            {
                await LoadAnnouncement();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "L'annonce a été supprimé", Duration = 4000 };
                NotificationService.Notify(message);
            }
            else
            {
                await LoadAnnouncement();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Une erreur s'est produite, l'annonce n'a pas été supprimé", Duration = 4000 };
                NotificationService.Notify(message);


            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des annonces : {ex.Message}");
        }
    }

    // appel pour l'affichage de l'update d'un event
    protected async Task OnUpdateAnnouncement(int id)
    {
        var announcementEnt = announcement.FirstOrDefault(a => a.Id == id);
        if (announcementEnt != null)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Announcement", announcementEnt}
            };

            var result = await DialogService.OpenAsync<UpdateAnnouncementMairie>("Modification de l'annonce", parameters);

            if (result != null && result)
            {
                await LoadAnnouncement();
                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "L'annonce a été modifié", Duration = 4000 };
                NotificationService.Notify(message);
            }
        }
        else
        {
            Console.WriteLine("announcement not found.");
        }
    }

}
