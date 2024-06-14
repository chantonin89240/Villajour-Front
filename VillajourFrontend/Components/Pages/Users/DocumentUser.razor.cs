using Microsoft.AspNetCore.Components;
using Radzen;
using System.Text.Json;
using System.Text;
using VillajourFrontend.Components.Pages.Mairie;
using VillajourFrontend.Dto;
using VillajourFrontend.Entity;

namespace VillajourFrontend.Components.Pages.Users;

public partial class DocumentUser
{
    [Inject]
    protected HttpClient? HttpClient { get; set; }

    [Inject]
    protected DialogService? DialogService { get; set; }

    [Inject]
    protected NavigationManager? NavigationManager { get; set; }

    [Inject] private NotificationService? NotificationService { get; set; }

    protected List<DocumentDto> documentsFav = new List<DocumentDto>();
    protected List<DocumentDto> documentsByMairieFav = new List<DocumentDto>();

    protected override async Task OnInitializedAsync()
    {
        await LoadDocuments();
    }

    protected async Task LoadDocuments()
    {
        var apiUrl = "https://localhost:7205/Api/Document/GetDocumentFav/3fa85f64-5717-4562-b3fc-2c963f66afa6";
        try
        {
            var documentMairie = await HttpClient.GetFromJsonAsync<List<DocumentDto>>(apiUrl);
            documentsFav = documentMairie?.ToList() ?? new List<DocumentDto>();
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
        var apiUrl = $"https://localhost:7205/Api/Document/DownloadDocument?fileUrl={Uri.EscapeDataString(url)}";
        NavigationManager.NavigateTo(apiUrl, true);
    }

    // suppression d'un document favoris
    protected async Task OnDeleteDocumentFav(int idDocument)
    {
        var apiUrl = "https://localhost:7205/Api/User/DeleteFavoriteContent/";
        DeleteFavoriteContentDto deleteFav = new DeleteFavoriteContentDto()
        {
            UserId = new Guid("3FA85F64-5717-4562-B3FC-2C963F66AFA6"),
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
                await LoadDocuments();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Le document a été supprimé", Duration = 10000 };
                NotificationService.Notify(message);
            }
            else
            {
                await LoadDocuments();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Une erreur s'est produite, le document n'a pas été supprimé", Duration = 10000 };
                NotificationService.Notify(message);


            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des documents : {ex.Message}");
        }
    }

    


}
