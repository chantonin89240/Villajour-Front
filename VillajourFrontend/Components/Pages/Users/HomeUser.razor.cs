using Microsoft.AspNetCore.Components;
using Radzen;
using System.Text.Json;
using System.Text;
using VillajourFrontend.Dto;
using VillajourFrontend.Dto.Event;

namespace VillajourFrontend.Components.Pages.Users;

public partial class HomeUser
{
    [Inject]
    protected HttpClient? HttpClient { get; set; }

    [Inject]
    protected DialogService? DialogService { get; set; }

    [Inject]
    protected NavigationManager? NavigationManager { get; set; }

    [Inject] private NotificationService? NotificationService { get; set; }

    List<VillajourFrontend.Entity.Mairie> mairies = new List<VillajourFrontend.Entity.Mairie>();
    List<VillajourFrontend.Entity.Mairie> filteredMairies = new List<VillajourFrontend.Entity.Mairie>();
    List<VillajourFrontend.Entity.Mairie> mairiesFav = new List<VillajourFrontend.Entity.Mairie>();
    string searchQuery = "";

    protected Guid userGuid => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

    protected override async Task OnInitializedAsync()
    {
        await LoadMairies();
        await LoadMairiesFav();
    }

    protected async Task LoadMairies()
    {
        var apiUrl = $"Mairie";
        try
        {
            var mairie = await HttpClient.GetFromJsonAsync<List<VillajourFrontend.Entity.Mairie>>(apiUrl);
            mairies = mairie?.ToList() ?? new List<VillajourFrontend.Entity.Mairie>();
            filteredMairies = mairies;
            this.StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des événements : {ex.Message}");
        }
    }

    protected async Task LoadMairiesFav()
    {
        var apiUrl = $"User/GetMairieFavByUser/{userGuid}";
        try
        {
            var mairie = await HttpClient.GetFromJsonAsync<List<VillajourFrontend.Entity.Mairie>>(apiUrl);
            mairiesFav = mairie?.ToList() ?? new List<VillajourFrontend.Entity.Mairie>();
            this.StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des événements : {ex.Message}");
        }
    }

    void FilterMairies()
    {
        if (string.IsNullOrEmpty(searchQuery))
        {
            filteredMairies = mairies;
        }
        else
        {
            filteredMairies = mairies.Where(m => m.Address != null && m.Address.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }

    // suppression d'un document favoris
    protected async Task OnDeleteMairieFav(Guid idMairie)
    {
        //var apiUrl = "User/DeleteFavoriteContent/";
        //FavoriteContentDto deleteFav = new FavoriteContentDto()
        //{
        //    UserId = userGuid,
        //    AnnouncementId = null,
        //    EventId = null,
        //    DocumentId = idDocument
        //};

        //try
        //{
        //    var request = new HttpRequestMessage(HttpMethod.Delete, apiUrl)
        //    {
        //        Content = new StringContent(JsonSerializer.Serialize(deleteFav), Encoding.UTF8, "application/json")
        //    };

        //    var response = await HttpClient.SendAsync(request);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        await LoadDocumentsFav();
        //        await LoadDocumentsFavMairie();

        //        NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Le document a été supprimé de vos favoris", Duration = 10000 };
        //        NotificationService.Notify(message);
        //    }
        //    else
        //    {
        //        await LoadDocumentsFav();
        //        await LoadDocumentsFavMairie();

        //        NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Une erreur s'est produite, le document n'a pas été supprimé", Duration = 10000 };
        //        NotificationService.Notify(message);


        //    }
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine($"Erreur lors du chargement des documents : {ex.Message}");
        //}
    }

}
