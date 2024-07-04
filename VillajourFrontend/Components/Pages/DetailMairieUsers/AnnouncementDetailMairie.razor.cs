using Microsoft.AspNetCore.Components;
using Radzen;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using VillajourFrontend.Dto;
using VillajourFrontend.Dto.Announcement;

namespace VillajourFrontend.Components.Pages.DetailMairieUsers;

public partial class AnnouncementDetailMairie
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

    protected List<AnnouncementByMairieDetailDto> announcementDetail = new List<AnnouncementByMairieDetailDto>();

    protected override async Task OnInitializedAsync()
    {
        await LoadAnnouncementDetail();
    }

    protected async Task LoadAnnouncementDetail()
    {
        var apiUrl = $"Announcement/GetAnnouncementByMairieDetail/{UserGuid}/{idMairie}";
        try
        {
            var announcementMairie = await HttpClient.GetFromJsonAsync<List<AnnouncementByMairieDetailDto>>(apiUrl);
            announcementDetail = announcementMairie?.ToList() ?? new List<AnnouncementByMairieDetailDto>();
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
            UserId = UserGuid,
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
                await LoadAnnouncementDetail();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "L'annonce a été ajouté à vos favoris", Duration = 4000 };
                NotificationService.Notify(message);
            }
            else
            {
                await LoadAnnouncementDetail();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Une erreur s'est produite, l'annonce n'a pas été ajouté", Duration = 4000 };
                NotificationService.Notify(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des annonces : {ex.Message}");
        }
    }

    // suppression d'un annonce favoris
    protected async Task OnDeleteAnnouncementFav(int idAnnouncement)
    {
        var apiUrl = "User/DeleteFavoriteContent/";
        FavoriteContentDto deleteFav = new FavoriteContentDto()
        {
            UserId = UserGuid,
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
                await LoadAnnouncementDetail();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "L'annonce a été retiré de vos favoris", Duration = 4000 };
                NotificationService.Notify(message);
            }
            else
            {
                await LoadAnnouncementDetail();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Une erreur s'est produite, l'annonce n'a pas été retiré", Duration = 4000 };
                NotificationService.Notify(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des annonces : {ex.Message}");
        }
    }
}
