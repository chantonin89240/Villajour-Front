using Microsoft.AspNetCore.Components;
using Radzen;
using VillajourFrontend.Dto.Event;
using VillajourFrontend.Entity;

namespace VillajourFrontend.Components.Pages.Mairie;

public partial class AddEventMairie : ComponentBase
{
    [Inject]
    protected HttpClient? HttpClient { get; set; }

    [Inject]
    protected DialogService? DialogService { get; set; }

    [Inject]
    protected NavigationManager? NavigationManager { get; set; }

    protected List<EventType> eventTypeList = new List<EventType>();

    protected AddEventDto newEvent = new AddEventDto();

    protected Guid MairieGuid => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

    protected override async Task OnInitializedAsync()
    {
        await LoadEventType();
    }

    protected async Task LoadEventType()
    {
        var apiUrl = "https://localhost:7205/Api/Event/GetEventType";
        try
        {
            var type = await HttpClient.GetFromJsonAsync<List<EventType>>(apiUrl);
            eventTypeList = type?.ToList() ?? new List<EventType>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des événements : {ex.Message}");
        }
    }

    protected async Task HandleValidSubmit()
    {
        var apiUrl = "https://localhost:7205/Api/Event";
        try
        {
            newEvent.MairieId = MairieGuid;
            var response = await HttpClient.PostAsJsonAsync(apiUrl, newEvent);
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