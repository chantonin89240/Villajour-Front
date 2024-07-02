using Microsoft.AspNetCore.Components;
using Radzen;
using VillajourFrontend.Dto.Event;
using VillajourFrontend.Entity;

namespace VillajourFrontend.Components.Pages.Mairie;

public partial class EventMairie
{
    [Inject]
    protected HttpClient? HttpClient { get; set; }

    [Inject]
    protected DialogService? DialogService { get; set; }

    [Inject]
    protected NavigationManager? NavigationManager { get; set; }

    [Inject] 
    private NotificationService? NotificationService { get; set; }

    protected List<EventDto> events = new List<EventDto>();

    protected Guid userGuid => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
    protected Guid mairieGuid => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

    protected override async Task OnInitializedAsync()
    {
        await LoadEvents();
    }

    protected async Task LoadEvents()
    {
        var apiUrl = $"Event/GetEventHistoByMairie/{mairieGuid}";
        try
        {
            var eventMairie = await HttpClient.GetFromJsonAsync<List<EventDto>>(apiUrl);
            events = eventMairie?.ToList() ?? new List<EventDto>();
            this.StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des événements : {ex.Message}");
        }
    }


    // redirige vers la page d'ajout d'un event
    private async void OnAddNewEvent()
    {
        var newEvent = new Event();
        var result = await DialogService.OpenAsync<AddEventMairie>("Ajouter un événement");

        if (result != null && result)
        {
            await LoadEvents();
            NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "L'événement a été ajouté", Duration = 4000 };
            NotificationService.Notify(message);
        }
    }

    // suppression d'un event
    protected async Task OnDeleteEvent(int id)
    {
        var apiUrl = $"Event/{id}";
        try
        {
            var validation = await HttpClient.DeleteAsync(apiUrl);

            if (validation.IsSuccessStatusCode)
            {
                await LoadEvents();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "L'événement a été supprimé", Duration = 4000 };
                NotificationService.Notify(message);
            }
            else
            {
                await LoadEvents();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Une erreur s'est produite, l'événement n'a pas été supprimé", Duration = 4000 };
                NotificationService.Notify(message);


            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des événements : {ex.Message}");
        }
    }

    // appel pour l'affichage de l'update d'un event
    protected async Task OnUpdateEvent(int id)
    {
        var eventEnt = events.FirstOrDefault(a => a.Id == id);
        if (eventEnt != null)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Event", eventEnt}
            };

            var result = await DialogService.OpenAsync<UpdateEventMairie>("Modification de l'événement", parameters);

            if (result != null && result)
            {
                await LoadEvents();
                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "L'événement a été modifié", Duration = 4000 };
                NotificationService.Notify(message);
            }
        }
        else
        {
            Console.WriteLine("event not found.");
        }
    }

    // appel pour l'affichage du détail d'un event
    protected async Task OnDetailEvent(int id)
    {
        var eventEnt = events.FirstOrDefault(a => a.Id == id);
        if (eventEnt != null)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Event", eventEnt}
            };

            await DialogService.OpenAsync<DetailEventMairie>("Détails de l'événement", parameters);
        }
        else
        {
            Console.WriteLine("event not found.");
        }
    }
}
