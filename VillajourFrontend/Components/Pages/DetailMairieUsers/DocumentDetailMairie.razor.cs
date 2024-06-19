using Microsoft.AspNetCore.Components;
using Radzen;
using System.Text;
using System.Text.Json;
using VillajourFrontend.Dto;
using VillajourFrontend.Dto.Document;

namespace VillajourFrontend.Components.Pages.DetailMairieUsers;

public partial class DocumentDetailMairie
{
    [Inject]
    protected HttpClient? HttpClient { get; set; }

    [Inject]
    protected DialogService? DialogService { get; set; }

    [Inject]
    protected NavigationManager? NavigationManager { get; set; }

    [Inject] private NotificationService? NotificationService { get; set; }


    protected List<DocumentByMairieDetailDto> documentsDetail = new List<DocumentByMairieDetailDto>();
    protected Guid userGuid => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
    protected Guid MairieGuid => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

    protected override async Task OnInitializedAsync()
    {
        await LoadDocumentsDetail();
    }

    protected async Task LoadDocumentsDetail()
    {
        var apiUrl = $"Document/GetDocumentByMairieDetail/{userGuid}/{MairieGuid}";
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
        var apiUrl = $"Document/DownloadDocument?fileUrl={Uri.EscapeDataString(url)}";
        NavigationManager.NavigateTo(apiUrl, true);
    }

    // Ajout d'un document favoris
    protected async Task OnAddDocumentFav(int idDocument)
    {
        var apiUrl = "User/AddFavoriteContent/";
        FavoriteContentDto addFav = new FavoriteContentDto()
        {
            UserId = userGuid,
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

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Le document a été ajouté à vos favoris", Duration = 10000 };
                NotificationService.Notify(message);
            }
            else
            {
                await LoadDocumentsDetail();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Une erreur s'est produite, le document n'a pas été ajouté", Duration = 10000 };
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
            UserId = userGuid,
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

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Le document a été retiré de vos favoris", Duration = 10000 };
                NotificationService.Notify(message);
            }
            else
            {
                await LoadDocumentsDetail();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Une erreur s'est produite, le document n'a pas été retiré", Duration = 10000 };
                NotificationService.Notify(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des documents : {ex.Message}");
        }
    }
}
