using Microsoft.AspNetCore.Components;
using Radzen;
using System.Text.Json;
using System.Text;
using VillajourFrontend.Dto;

namespace VillajourFrontend.Components.Pages.Mairie;

public partial class HomeMairie
{
    [Inject]
    protected HttpClient? HttpClient { get; set; }

    [Inject]
    protected DialogService? DialogService { get; set; }

    [Inject]
    protected NavigationManager? NavigationManager { get; set; }

    [Inject]
    private NotificationService? NotificationService { get; set; }

    HomeMairieDto homeMairie = new HomeMairieDto();

    protected Guid mairieGuid => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

    protected override async Task OnInitializedAsync()
    {
        await LoadHomeMairie();
    }

    protected async Task LoadHomeMairie()
    {
        var apiUrl = $"Mairie/GetHomeMairie/{mairieGuid}";
        try
        {
            homeMairie = await HttpClient.GetFromJsonAsync<HomeMairieDto>(apiUrl);
            this.StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des événements : {ex.Message}");
        }
    }
}
