using Microsoft.AspNetCore.Components;
using Radzen;
using System.Text.Json;
using System.Text;
using VillajourFrontend.Dto.Document;
using VillajourFrontend.Dto;
using VillajourFrontend.Dto.Event;
using VillajourFrontend.Components.Pages.Users;

namespace VillajourFrontend.Components.Pages.DetailMairieUsers;

public partial class EventDetailMairie
{
    [Inject]
    protected HttpClient? HttpClient { get; set; }

    [Inject]
    protected DialogService? DialogService { get; set; }

    [Inject]
    protected NavigationManager? NavigationManager { get; set; }

    [Inject] private NotificationService? NotificationService { get; set; }

    protected List<EventByMairieDetailDto> eventsDetail = new List<EventByMairieDetailDto>();
    protected Guid userGuid => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
    protected Guid MairieGuid => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

    protected override async Task OnInitializedAsync()
    {
        await LoadEventsDetail();
    }

    protected async Task LoadEventsDetail()
    {
        var apiUrl = $"Event/GetEventByMairieDetail/{userGuid}/{MairieGuid}";
        try
        {
            var eventMairie = await HttpClient.GetFromJsonAsync<List<EventByMairieDetailDto>>(apiUrl);
            eventsDetail = eventMairie?.ToList() ?? new List<EventByMairieDetailDto>();
            this.StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des événements : {ex.Message}");
        }
    }

    // Ajout d'un event favoris
    protected async Task OnAddEventFav(int idEvent)
    {
        var apiUrl = "User/AddFavoriteContent/";
        FavoriteContentDto addFav = new FavoriteContentDto()
        {
            UserId = userGuid,
            AnnouncementId = null,
            EventId = idEvent,
            DocumentId = null
        };

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
            {
                Content = new StringContent(JsonSerializer.Serialize(addFav), Encoding.UTF8, "application/json")
            };

            var response = await HttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                await LoadEventsDetail();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "L'événement a été ajouté à vos favoris", Duration = 10000 };
                NotificationService.Notify(message);
            }
            else
            {
                await LoadEventsDetail();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Une erreur s'est produite, l'événement n'a pas été ajouté", Duration = 10000 };
                NotificationService.Notify(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des documents : {ex.Message}");
        }
    }

    // suppression d'un event favoris
    protected async Task OnDeleteEventFav(int idEvent)
    {
        var apiUrl = "User/DeleteFavoriteContent/";
        FavoriteContentDto deleteFav = new FavoriteContentDto()
        {
            UserId = userGuid,
            AnnouncementId = null,
            EventId = idEvent,
            DocumentId = null
        };

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, apiUrl)
            {
                Content = new StringContent(JsonSerializer.Serialize(deleteFav), Encoding.UTF8, "application/json")
            };

            var response = await HttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                await LoadEventsDetail();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "L'événement a été retiré de vos favoris", Duration = 10000 };
                NotificationService.Notify(message);
            }
            else
            {
                await LoadEventsDetail();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Une erreur s'est produite, l'événement n'a pas été retiré", Duration = 10000 };
                NotificationService.Notify(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des documents : {ex.Message}");
        }
    }

    // appel pour l'affichage du détail d'un event
    protected async Task OnDetailEventFav(int id)
    {
        EventByMairieDetailDto eventEnt = eventsDetail.FirstOrDefault(a => a.Id == id);

        if (eventEnt != null)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Event", eventEnt}
            };

            await DialogService.OpenAsync<DetailEvent>("Détails de l'événement", parameters);
        }
        else
        {
            Console.WriteLine("event not found.");
        }
    }
}
