using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using VillajourFrontend.Dto;
using VillajourFrontend.Dto.Document;

namespace VillajourFrontend.Components.Pages.DetailMairieUsers;

public partial class DocumentDetailMairie
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
    private IJSRuntime JS { get; set; }

    [Inject]
    private IHttpContextAccessor? _httpContext { get; set; }
    protected Guid UserGuid { get => new Guid(_httpContext?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value); }

    protected List<DocumentByMairieDetailDto> documentsDetail = new List<DocumentByMairieDetailDto>();

    protected override async Task OnInitializedAsync()
    {
        await LoadDocumentsDetail();
    }

    protected async Task LoadDocumentsDetail()
    {
        var apiUrl = $"Document/GetDocumentByMairieDetail/{UserGuid}/{idMairie}";
        try
        {
            var documentMairie = await HttpClient.GetFromJsonAsync<List<DocumentByMairieDetailDto>>(apiUrl);
            documentsDetail = documentMairie?.ToList() ?? new List<DocumentByMairieDetailDto>();
            this.StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des documents : {ex.Message}");
        }
    }

    // download du document
    protected async Task OnDownloadDocument(string url)
    {
        var uri = new Uri(url, true);

        var apiUrl = $"Document/DownloadDocument?fileUrl={Uri.EscapeDataString(url)}";

        var fileStream = await HttpClient.GetStreamAsync(apiUrl);
        var fileName = uri.Segments.Last();

        using var streamRef = new DotNetStreamReference(stream: fileStream);

        await JS.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
    }

    // Ajout d'un document favoris
    protected async Task OnAddDocumentFav(int idDocument)
    {
        var apiUrl = "User/AddFavoriteContent/";
        FavoriteContentDto addFav = new FavoriteContentDto()
        {
            UserId = UserGuid,
            AnnouncementId = null,
            EventId = null,
            DocumentId = idDocument
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
                await LoadDocumentsDetail();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Le document a été ajouté à vos favoris", Duration = 4000 };
                NotificationService.Notify(message);
            }
            else
            {
                await LoadDocumentsDetail();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Une erreur s'est produite, le document n'a pas été ajouté", Duration = 4000 };
                NotificationService.Notify(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des documents : {ex.Message}");
        }
    }

    // suppression d'un document favoris
    protected async Task OnDeleteDocumentFav(int idDocument)
    {
        var apiUrl = "User/DeleteFavoriteContent/";
        FavoriteContentDto deleteFav = new FavoriteContentDto()
        {
            UserId = UserGuid,
            AnnouncementId = null,
            EventId = null,
            DocumentId = idDocument
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
                await LoadDocumentsDetail();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Le document a été retiré de vos favoris", Duration = 4000 };
                NotificationService.Notify(message);
            }
            else
            {
                await LoadDocumentsDetail();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Une erreur s'est produite, le document n'a pas été retiré", Duration = 4000 };
                NotificationService.Notify(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des documents : {ex.Message}");
        }
    }
}
