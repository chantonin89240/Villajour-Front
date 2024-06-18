using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using System;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VillajourFrontend.Components.Pages.Mairie
{
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

        protected VillajourFrontend.Entity.Mairie model = new VillajourFrontend.Entity.Mairie();


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
            var apiUrl = "https://localhost:44357/Api/Mairie/" + MairieId;
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
        
        protected async Task LoadPopup()
        {
            var result = await DialogService.OpenAsync<DeleteAccount>("Suppression du compte");

            if (result != null)
            {
                await scheduler.Reload();
            }
        }

        protected async Task ModifyMairie(VillajourFrontend.Entity.Mairie model)
        {
            var apiUrl = "https://localhost:44357/Api/Mairie";
            var apiModel = new Entity.Mairie
            {
                Id = MairieId,
                Address = model.Address,
                Phone = model.Phone,
                Picture = model.Picture,
                Siret = model.Siret
            };

            try
            {
                HttpResponseMessage response;
                { 
                    response = await HttpClient.PutAsJsonAsync($"{apiUrl}/{MairieId}", apiModel);
                }
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la modification de la mairie: {ex.Message}");
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
            await LoadMairie();
        }
    }   
}