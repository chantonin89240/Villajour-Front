using Microsoft.AspNetCore.Components;
using Radzen;
using VillajourFrontend.Entity;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using VillajourFrontend.Dto.Appointement;

namespace VillajourFrontend.Components
{
    public partial class AppointmentFormBase : ComponentBase
    {
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected HttpClient HttpClient { get; set; }

        [Parameter]
        public Appointment CurrentAppointment { get; set; }

        [Parameter]
        public string Title { get; set; }

        protected Appointment model = new Appointment();
        protected List<AppointmentTypeDTO> appointmentTypes = new List<AppointmentTypeDTO>();

        protected Guid UserId => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
        protected Guid MairieId => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

        protected override async Task OnInitializedAsync()
        {
            await LoadAppointmentTypes();
            if (CurrentAppointment != null)
            {
                model = CurrentAppointment;
            }
        }

        protected async Task LoadAppointmentTypes()
        {
            var apiUrl = "Appointments/GetAppointmentType";
            try
            {
                var types = await HttpClient.GetFromJsonAsync<List<AppointmentTypeDTO>>(apiUrl);
                appointmentTypes = types ?? new List<AppointmentTypeDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des types de rendez-vous: {ex.Message}");
            }
        }

        protected async Task OnSubmit()
        {
            if (string.IsNullOrEmpty(model.Title))
            {
                return;
            }

            model.UserId = UserId;
            model.MairieId = MairieId;
            model.Statut = "en cours";

            var apiUrl = model.Id == 0 ? "Appointments" : $"Appointments/{model.Id}";
            Console.WriteLine($"API URL: {apiUrl}");

            try
            {
                HttpResponseMessage response;
                if (model.Id == 0)
                {
                    response = await HttpClient.PostAsJsonAsync(apiUrl, model);
                }
                else
                {
                    response = await HttpClient.PutAsJsonAsync(apiUrl, model);
                }
                response.EnsureSuccessStatusCode();
                DialogService.Close(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la gestion du rendez-vous: {ex.Message}");
            }
        }


        protected async Task RefuseAppointment()
        {
            if (model.Id == 0)
            {
                DialogService.Close(null);
            };
            var apiUrl = $"Appointments/{model.Id}";

            try
            {
                var response = await HttpClient.PutAsJsonAsync(apiUrl, model);
                response.EnsureSuccessStatusCode();
                DialogService.Close(null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la mise à jour du statut du rendez-vous: {ex.Message}");
            }
        }
    }
}
