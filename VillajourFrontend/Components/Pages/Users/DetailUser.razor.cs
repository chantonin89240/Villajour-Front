using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using System;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VillajourFrontend.Components.Pages.Users
{
    public partial class DetailUser : ComponentBase
    {
        [Inject]
        protected HttpClient HttpClient { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        protected VillajourFrontend.Entity.User model = new VillajourFrontend.Entity.User();


        protected RadzenScheduler<Entity.User> scheduler;
        protected Guid UserId => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
        protected bool IsUser => true;

        protected bool NotModificationMode;

        protected override async Task OnInitializedAsync()
        {
            NotModificationMode = true;
            await LoadUser();
        }

        protected async Task LoadUser()
        {
            var apiUrl = "https://localhost:44357/Api/User/" + UserId;
            try
            {
                var apiUser = await HttpClient.GetFromJsonAsync<Entity.User>(apiUrl);

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

        protected async Task LoadPopup()
        {
            var result = await DialogService.OpenAsync<DeleteAccount>("Suppression du compte");

            if (result != null)
            {
                await scheduler.Reload();
            }
        }

        protected async Task ModifyUser(VillajourFrontend.Entity.User model)
        {
            var apiUrl = "https://localhost:44357/Api/User";
            var apiModel = new Entity.User
            {
                Id = UserId,
                Phone = model.Phone,
                Picture = model.Picture,
            };

            try
            {
                HttpResponseMessage response;
                {
                    response = await HttpClient.PutAsJsonAsync($"{apiUrl}/{UserId}", apiModel);
                }
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la modification de la user: {ex.Message}");
            }

            NotModificationMode = true;

            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Duration = 4000,
                Summary = "Modifications enregistrées avec succès",
                Detail = ""
            });
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
}