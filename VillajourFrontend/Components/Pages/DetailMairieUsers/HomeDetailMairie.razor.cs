using Microsoft.AspNetCore.Components;
using Radzen;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using VillajourFrontend.Dto;

namespace VillajourFrontend.Components.Pages.DetailMairieUsers;

public partial class HomeDetailMairie
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

    protected DetailMairieDto detailMairie = new DetailMairieDto();

    protected override async Task OnInitializedAsync()
    {
        await LoadDetailMairie();
    }

    protected async Task LoadDetailMairie()
    {
        var apiUrl = $"Mairie/GetDetailMairie/{UserGuid}/{idMairie}";
        try
        {
            detailMairie = await HttpClient.GetFromJsonAsync<DetailMairieDto>(apiUrl);
            this.StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des événements : {ex.Message}");
        }
    }

    // ajout d'une mairie favoris
    protected async Task OnAddMairieFav(Guid idMairie)
    {
        var apiUrl = "User/AddFavoriteMairie";
        FavoriteMairieDto deleteFav = new FavoriteMairieDto()
        {
            UserId = UserGuid,
            MairieId = idMairie
        };

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
            {
                Content = new StringContent(JsonSerializer.Serialize(deleteFav), Encoding.UTF8, "application/json")
            };

            var response = await HttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                await LoadDetailMairie();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "La mairie a été ajouté à vos favoris", Duration = 4000 };
                NotificationService.Notify(message);
            }
            else
            {
                await LoadDetailMairie();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Une erreur s'est produite, la mairie n'a pas été ajouté", Duration = 4000 };
                NotificationService.Notify(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des documents : {ex.Message}");
        }
    }

    // suppression d'une mairie favoris
    protected async Task OnDeleteMairieFav(Guid idMairie)
    {
        var apiUrl = "User/DeleteFavoriteMairie/";
        FavoriteMairieDto deleteFav = new FavoriteMairieDto()
        {
            UserId = UserGuid,
            MairieId = idMairie
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
                await LoadDetailMairie();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "La mairie a été supprimé de vos favoris", Duration = 4000 };
                NotificationService.Notify(message);
            }
            else
            {
                await LoadDetailMairie();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Une erreur s'est produite, la mairie n'a pas été supprimé", Duration = 4000 };
                NotificationService.Notify(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des documents : {ex.Message}");
        }
    }
}
