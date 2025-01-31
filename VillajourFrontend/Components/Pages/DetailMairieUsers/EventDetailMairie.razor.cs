﻿using Microsoft.AspNetCore.Components;
using Radzen;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using VillajourFrontend.Dto;
using VillajourFrontend.Dto.Event;

namespace VillajourFrontend.Components.Pages.DetailMairieUsers;

public partial class EventDetailMairie
{    
    [Parameter]
    public Guid idMairie { get; set; }

    [Inject]
    protected HttpClient? HttpClient { get; set; }

    [Inject]
    protected DialogService? DialogService { get; set; }

    [Inject]
    protected NavigationManager? NavigationManager { get; set; }

    [Inject] 
    private NotificationService? NotificationService { get; set; }

    [Inject]
    private IHttpContextAccessor? _httpContext { get; set; }
    protected Guid UserGuid { get => new Guid(_httpContext?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value); }

    protected List<EventByMairieDetailDto> eventsDetail = new List<EventByMairieDetailDto>();

    protected override async Task OnInitializedAsync()
    {
        await LoadEventsDetail();
    }

    protected async Task LoadEventsDetail()
    {
        var apiUrl = $"Event/GetEventByMairieDetail/{UserGuid}/{idMairie}";
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
            UserId = UserGuid,
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

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "L'événement a été ajouté à vos favoris", Duration = 4000 };
                NotificationService.Notify(message);
            }
            else
            {
                await LoadEventsDetail();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Une erreur s'est produite, l'événement n'a pas été ajouté", Duration = 4000 };
                NotificationService.Notify(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des événements : {ex.Message}");
        }
    }

    // suppression d'un event favoris
    protected async Task OnDeleteEventFav(int idEvent)
    {
        var apiUrl = "User/DeleteFavoriteContent/";
        FavoriteContentDto deleteFav = new FavoriteContentDto()
        {
            UserId = UserGuid,
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

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "L'événement a été retiré de vos favoris", Duration = 4000 };
                NotificationService.Notify(message);
            }
            else
            {
                await LoadEventsDetail();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Une erreur s'est produite, l'événement n'a pas été retiré", Duration = 4000 };
                NotificationService.Notify(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des événements : {ex.Message}");
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
