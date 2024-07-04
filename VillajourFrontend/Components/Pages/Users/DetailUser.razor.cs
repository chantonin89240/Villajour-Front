using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using System.Security.Claims;
using VillajourFrontend.Entity;

namespace VillajourFrontend.Components.Pages.Users;

public partial class DetailUser : ComponentBase
{
    [Inject]
    protected HttpClient? HttpClient { get; set; }

    [Inject]
    protected DialogService? DialogService { get; set; }

    [Inject]
    protected NavigationManager? NavigationManager { get; set; }

    [Inject]
    protected NotificationService? NotificationService { get; set; }

    [Inject]
    private IHttpContextAccessor? _httpContext { get; set; }
    protected Guid userGuid { get => new Guid(_httpContext?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value); }

    protected User model = new User();

    protected RadzenScheduler<User>? scheduler;
    protected bool IsUser => true;

    protected bool NotModificationMode;

    protected override async Task OnInitializedAsync()
    {
        NotModificationMode = true;
        await LoadUser();
    }

    protected async Task LoadUser()
    {
        var apiUrl = "User/" + userGuid;
        try
        {
            var apiUser = await HttpClient.GetFromJsonAsync<User>(apiUrl);

            if (apiUser != null)
            {
                model = apiUser;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement de la user: {ex.Message}");
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

    protected async Task ModifyUser(User model)
    {
        try
        {
            var apiUrl = "User/" + model.Id;

            var apiModel = new User
            {
                Id = userGuid,
                Phone = model.Phone,
                Picture = model.Picture,
                Email = model.Email,
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
        await LoadUser();
    }
}