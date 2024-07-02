using Microsoft.AspNetCore.Components;
using Radzen;
using System.Reflection;
using System.Text.Json;
using System.Text;
using VillajourFrontend.Components.Pages.DetailMairieUsers;
using VillajourFrontend.Dto;
using VillajourFrontend.Dto.Announcement;
using VillajourFrontend.Dto.Event;
using VillajourFrontend.Entity;

namespace VillajourFrontend.Components.Pages.Users;

public partial class AnnouncementUser
{
    [Inject]
    protected HttpClient? HttpClient { get; set; }

    [Inject]
    protected DialogService? DialogService { get; set; }

    [Inject]
    protected NavigationManager? NavigationManager { get; set; }

    [Inject] private NotificationService? NotificationService { get; set; }

    protected List<AnnouncementDto> annonceFav = new List<AnnouncementDto>();
    protected List<AnnouncementByMairieFavoriteDto> annonceByMairieFav = new List<AnnouncementByMairieFavoriteDto>();

    protected Guid userGuid => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
    protected Guid mairieGuid => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

    protected override async Task OnInitializedAsync()
    {
        await LoadAnnouncementFav();
        await LoadAnnouncementFavMairie();
    }

    protected async Task LoadAnnouncementFav()
    {
        var apiUrl = $"Announcement/GetAnnouncementFavoriteByUser/{userGuid}";
        try
        {
            var annonceMairie = await HttpClient.GetFromJsonAsync<List<AnnouncementDto>>(apiUrl);
            annonceFav = annonceMairie?.ToList() ?? new List<AnnouncementDto>();
            this.StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des annonces : {ex.Message}");
        }
    }

    protected async Task LoadAnnouncementFavMairie()
    {
        var apiUrl = $"Announcement/GetAnnouncementByMairieFavorite/{userGuid}";
        try
        {
            var annonceMairie = await HttpClient.GetFromJsonAsync<List<AnnouncementByMairieFavoriteDto>>(apiUrl);
            annonceByMairieFav = annonceMairie?.ToList() ?? new List<AnnouncementByMairieFavoriteDto>();
            this.StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des annonces : {ex.Message}");
        }
    }

    // Ajout d'une annonce favoris
    protected async Task OnAddAnnouncementFav(int idAnnouncement)
    {
        var apiUrl = "User/AddFavoriteContent/";
        FavoriteContentDto addFav = new FavoriteContentDto()
        {
            UserId = userGuid,
            AnnouncementId = idAnnouncement,
            EventId = null,
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
                await LoadAnnouncementFav();
                await LoadAnnouncementFavMairie();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "L'annonce a été ajouté à vos favoris", Duration = 4000 };
                NotificationService.Notify(message);
            }
            else
            {
                await LoadAnnouncementFav();
                await LoadAnnouncementFavMairie();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Une erreur s'est produite, l'annonce n'a pas été ajouté", Duration = 4000 };
                NotificationService.Notify(message);


            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des événements : {ex.Message}");
        }
    }

    // suppression d'une annonce favoris
    protected async Task OnDeleteAnnouncementFav(int idAnnouncement)
    {
        var apiUrl = "User/DeleteFavoriteContent/";
        FavoriteContentDto deleteFav = new FavoriteContentDto()
        {
            UserId = userGuid,
            AnnouncementId = idAnnouncement,
            EventId = null,
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
                await LoadAnnouncementFav();
                await LoadAnnouncementFavMairie();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "L'annonce a été supprimé de vos favoris", Duration = 4000 };
                NotificationService.Notify(message);
            }
            else
            {
                await LoadAnnouncementFav();
                await LoadAnnouncementFavMairie();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Une erreur s'est produite, l'annonce n'a pas été supprimé", Duration = 4000 };
                NotificationService.Notify(message);


            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des annonces : {ex.Message}");
        }
    }

    // appel pour l'affichage du détail d'une annonce favoris
    protected async Task OnDetailAnnouncementFav(int id, bool favAnnonce)
    {
        AnnouncementDto annonceEnt = null;
        if (favAnnonce)
        {
            annonceEnt = annonceFav.FirstOrDefault(a => a.Id == id);
        }
        else
        {
            var eve = annonceByMairieFav.FirstOrDefault(a => a.Id == id);

            AnnouncementDto annonceRecup = new AnnouncementDto()
            {
                Id = eve.Id,
                Mairie = eve.Mairie,
                AnnouncementType = eve.AnnouncementType,
                Description = eve.Description,
                Date = eve.Date,
                Title = eve.Title,
            };

            annonceEnt = annonceRecup;
        }

        if (annonceEnt != null)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Announcement", annonceEnt}
            };

            await DialogService.OpenAsync<DetailAnnouncementUser>("Détails de la mairie", parameters);
        }
        else
        {
            Console.WriteLine("announcement not found.");
        }
    }

}
