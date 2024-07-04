using Microsoft.AspNetCore.Components;
using Radzen;
using System.Security.Claims;
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

    [Inject]
    private IHttpContextAccessor? _httpContext { get; set; }
    protected Guid MairieGuid { get => new Guid(_httpContext?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value); }

    HomeMairieDto homeMairie = new HomeMairieDto();

    protected override async Task OnInitializedAsync()
    {
        await LoadHomeMairie();
    }

    protected async Task LoadHomeMairie()
    {
        var apiUrl = $"Mairie/GetHomeMairie/{MairieGuid}";
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
