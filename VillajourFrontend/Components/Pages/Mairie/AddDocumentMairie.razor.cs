using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using System.Net.Http.Headers;
using VillajourFrontend.Dto.Document;
using VillajourFrontend.Entity;

namespace VillajourFrontend.Components.Pages.Mairie;

public partial class AddDocumentMairie : ComponentBase
{
    [Inject]
    protected HttpClient? HttpClient { get; set; }

    [Inject]
    protected DialogService? DialogService { get; set; }

    [Inject]
    protected NavigationManager? NavigationManager { get; set; }

    protected List<DocumentType> documentTypeList = new List<DocumentType>();

    protected AddDocumentDto newDocument = new AddDocumentDto();
    private IBrowserFile? selectedFile;

    protected Guid userGuid => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
    protected Guid MairieGuid => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

    protected override async Task OnInitializedAsync()
    {
        await LoadDocumentType();
    }

    protected async Task LoadDocumentType()
    {
        var apiUrl = "Document/GetDocumentType";
        try
        {
            var type = await HttpClient.GetFromJsonAsync<List<DocumentType>>(apiUrl);
            documentTypeList = type?.ToList() ?? new List<DocumentType>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des rendez-vous: {ex.Message}");
        }
    }

    protected async Task HandleValidSubmit()
    {
        var apiUrl = "Document";
        try
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(newDocument.Title), "Title");

            string descriptionDoc;
            if (newDocument.Description == null)
            {
                descriptionDoc = "";
            }
            else
            {
                descriptionDoc = newDocument.Description;
            }
            content.Add(new StringContent(descriptionDoc), "Description");
            content.Add(new StringContent(newDocument.DocumentTypeId.ToString()), "DocumentTypeId");
            content.Add(new StringContent(MairieGuid.ToString()), "MairieId");

            if (selectedFile != null)
            {
                var stream = selectedFile.OpenReadStream();
                var streamContent = new StreamContent(stream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(selectedFile.ContentType);
                content.Add(streamContent, "Document", selectedFile.Name);

                var response = await HttpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    // Le document a été ajouté avec succès, fermer la boîte de dialogue et actualiser la liste des documents
                    DialogService.Close(true);
                }
                else
                {
                    // Gestion des erreurs de l'API
                    Console.WriteLine($"Erreur lors de l'ajout du document: {response.StatusCode}");
                }            
            }
            else
            {
                Console.WriteLine($"Le document est vide !");
            }
        }
        catch (Exception ex)
        {
            // Gestion des erreurs du client
            Console.WriteLine($"Erreur lors de l'ajout du document : {ex.Message}");
        }
    }

    async Task OnClientChange(UploadChangeEventArgs args)
    {
        selectedFile = args.Files.FirstOrDefault();
    }
}