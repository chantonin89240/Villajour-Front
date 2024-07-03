using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using VillajourFrontend.Dto.Document;
using VillajourFrontend.Entity;

namespace VillajourFrontend.Components.Pages.Mairie;

public partial class DocumentMairie
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
    private IJSRuntime JS { get; set; }

    protected List<DocumentDto> documents = new List<DocumentDto>();

    protected Guid userGuid => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
    protected Guid mairieGuid => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

    protected override async Task OnInitializedAsync()
    {
        await LoadDocuments();
    }

    protected async Task LoadDocuments()
    {
        var apiUrl = $"Document/GetDocumentHistoByMairie/{mairieGuid}";
        try
        {
            var documentMairie = await HttpClient.GetFromJsonAsync<List<DocumentDto>>(apiUrl);
            documents = documentMairie?.ToList() ?? new List<DocumentDto>();
            this.StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des documents : {ex.Message}");
        }
    }

    // redirige vers la page d'ajout de document
    private async void OnAddNewDocument()
    {
        var newDocument = new Document();
        var result = await DialogService.OpenAsync<AddDocumentMairie>("Ajouter un document");

        if (result != null && result)
        {
            await LoadDocuments();
            NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Le document a été ajouté", Duration = 4000 };
            NotificationService.Notify(message);
        }
    }

    // suppression d'un document
    protected async Task OnDeleteDocument(int id)
    {
        var apiUrl = "Document/" + id;
        try
        {
            var validation = await HttpClient.DeleteAsync(apiUrl);

            if (validation.IsSuccessStatusCode)
            {
                await LoadDocuments();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Le document a été supprimé", Duration = 4000 };
                NotificationService.Notify(message);
            }
            else
            {
                await LoadDocuments();

                NotificationMessage message = new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Une erreur s'est produite, le document n'a pas été supprimé", Duration = 4000 };
                NotificationService.Notify(message);

                
            }
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

}
