using Microsoft.AspNetCore.Components;
using Radzen;
using System.Text;
using System.Text.Json;
using VillajourFrontend.Dto;
using VillajourFrontend.Dto.Event;

namespace VillajourFrontend.Components.Pages.Users;

public partial class EventUser
{
    [Inject]
    protected HttpClient? HttpClient { get; set; }

    [Inject]
    protected DialogService? DialogService { get; set; }

    [Inject]
    protected NavigationManager? NavigationManager { get; set; }

    [Inject] private NotificationService? NotificationService { get; set; }

    protected List<EventDto> eventsFav = new List<EventDto>();
    protected List<EventByMairieFavoriteDto> eventsByMairieFav = new List<EventByMairieFavoriteDto>();

    protected Guid userGuid => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
    protected Guid mairieGuid => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

    protected override async Task OnInitializedAsync()
    {
        await LoadEventsFav();
        await LoadEventsFavMairie();
    }

    protected async Task LoadEventsFav()
    {
        var apiUrl = $"Event/GetEventFavoriteByUser/{userGuid}";
        try
        {
            var eventMairie = await HttpClient.GetFromJsonAsync<List<EventDto>>(apiUrl);
            eventsFav = eventMairie?.ToList() ?? new List<EventDto>();
            this.StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des événements : {ex.Message}");
        }
    }

    protected async Task LoadEventsFavMairie()
    {
        var apiUrl = $"Event/GetEventByMairieFavorite/{userGuid}";
        try
        {
            var eventMairie = await HttpClient.GetFromJsonAsync<List<EventByMairieFavoriteDto>>(apiUrl);
            eventsByMairieFav = eventMairie?.ToList() ?? new List<EventByMairieFavoriteDto>();
            this.StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des événements : {ex.Message}");
        }
    }

    // Ajout d'un événement favoris
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
                await LoadEventsFav();
                await LoadEventsFavMairie();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "L'événement a été ajouté à vos favoris", Duration = 4000 };
                NotificationService.Notify(message);
            }
            else
            {
                await LoadEventsFav();
                await LoadEventsFavMairie();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Une erreur s'est produite, l'événement n'a pas été ajouté", Duration = 4000 };
                NotificationService.Notify(message);


            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des événements : {ex.Message}");
        }
    }

    // suppression d'un événement favoris
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
                await LoadEventsFav();
                await LoadEventsFavMairie();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "L'événement a été supprimé de vos favoris", Duration = 4000 };
                NotificationService.Notify(message);
            }
            else
            {
                await LoadEventsFav();
                await LoadEventsFavMairie();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Une erreur s'est produite, l'événement n'a pas été supprimé", Duration = 4000 };
                NotificationService.Notify(message);


            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des événements : {ex.Message}");
        }
    }

    // appel pour l'affichage du détail d'un event favoris
    protected async Task OnDetailEventFav(int id, bool favEvent)
    {
        EventDto eventEnt = null;
        if (favEvent)
        {
            eventEnt = eventsFav.FirstOrDefault(a => a.Id == id);
        }
        else 
        {
            var eve = eventsByMairieFav.FirstOrDefault(a => a.Id == id);

            EventDto eventRecup = new EventDto()
            {
                Id = eve.Id,
                Mairie = eve.Mairie,
                Address = eve.Address,
                Description = eve.Description,
                EndTime = eve.EndTime,
                EventType = eve.EventType,
                StartTime = eve.StartTime,  
                Title = eve.Title,
            };

            eventEnt = eventRecup;
        }

        if (eventEnt != null)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Event", eventEnt}
            };

            await DialogService.OpenAsync<DetailEventFav>("Détails de l'événement", parameters);
        }
        else
        {
            Console.WriteLine("event not found.");
        }
    }
}
