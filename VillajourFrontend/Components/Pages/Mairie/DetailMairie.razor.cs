using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace VillajourFrontend.Components.Pages.Mairie;

public partial class DetailMairie : ComponentBase
{
    [Inject]
    protected HttpClient HttpClient { get; set; }

    [Inject]
    protected DialogService DialogService { get; set; }

    [Inject]
    protected NavigationManager NavigationManager { get; set; }

    [Inject]
    protected NotificationService NotificationService { get; set; }

    protected Entity.Mairie model = new Entity.Mairie();


    protected RadzenScheduler<Entity.Mairie> scheduler;
    protected int UserId => 1;
    protected Guid MairieId => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
    protected bool IsMairie => true;

    protected bool NotModificationMode;

    protected override async Task OnInitializedAsync()
    {
        NotModificationMode = true;
        await LoadMairie();
    }

    protected async Task LoadMairie()
    {
        var apiUrl = "Mairie/" + MairieId;
        try
        {
            var apiMairie = await HttpClient.GetFromJsonAsync<Entity.Mairie>(apiUrl);

            if (apiMairie != null) 
            {
                model = apiMairie;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement de la mairie: {ex.Message}");
        }
    }

    protected async Task PopupDeleteAccount()
    {
        var result = await DialogService.OpenAsync<DeleteAccount>("Suppression du compte");

            if (result != null)
            {
                await scheduler.Reload();
            }
        }

        protected async Task PopupContact()
        {
            var result = await DialogService.OpenAsync<Contact>("Nous contacter");

        if (result != null)
        {
            await scheduler.Reload();
        }
    }

    protected async Task ModifyMairie(Entity.Mairie model)
    {
        try
        {
            var apiUrl = "Mairie/" + model.Id;

            var apiModel = new Entity.Mairie
            {
                Id = MairieId,
                Name = model.Name,
                Address = model.Address,
                Phone = model.Phone,
                Picture = model.Picture,
                Email = model.Email,
                Siret = model.Siret
            };

            var response = await HttpClient.PutAsJsonAsync(apiUrl, apiModel);
            if (response.IsSuccessStatusCode)
            {
                NotModificationMode = true;

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Duration = 4000,
                    Summary = "Modifications enregistrées avec succès",
                    Detail = ""
                });
            }
            else
            {
                NotModificationMode = true;

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Warning,
                    Duration = 4000,
                    Summary = "La modification a échouer",
                    Detail = ""
                });
                Console.WriteLine($"Erreur lors de la modification du profil : {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            // Gestion des erreurs du client
            Console.WriteLine($"Erreur lors de la modification du profil : {ex.Message}");
        }
    }

    protected void EnterModificationMode()
    {
        NotModificationMode = false;
    }

    protected async Task ExitModificationMode()
    {
        NotModificationMode = true;
        await LoadMairie();
    }
}   