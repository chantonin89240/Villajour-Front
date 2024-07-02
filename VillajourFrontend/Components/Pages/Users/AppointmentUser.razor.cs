using Microsoft.AspNetCore.Components;
using Radzen;
using System.Net.Http;
using System.Net.Http.Json;
using VillajourFrontend.Dto.Appointement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace VillajourFrontend.Components.Pages.Users
{
    public partial class AppointmentUserBase : ComponentBase
    {
        [Inject]
        protected HttpClient HttpClient { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        protected List<AppointmentDTO> PastAppointments { get; set; } = new List<AppointmentDTO>();
        protected List<AppointmentDTO> FutureAppointments { get; set; } = new List<AppointmentDTO>();
        protected List<AppointmentTypeDTO> AppointmentTypes { get; set; } = new List<AppointmentTypeDTO>();

        protected Guid userGuid => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

        protected override async Task OnInitializedAsync()
        {
            await LoadAppointments();
        }

        private async Task LoadAppointments()
        {
            var apiUrlAppointments = $"Appointments/GetAppointmentByUser/{userGuid}";
            var apiUrlAppointmentTypes = "Appointments/GetAppointmentType";
            try
            {
                var appointments = await HttpClient.GetFromJsonAsync<List<AppointmentDTO>>(apiUrlAppointments);
                AppointmentTypes = await HttpClient.GetFromJsonAsync<List<AppointmentTypeDTO>>(apiUrlAppointmentTypes);

                if (appointments != null)
                {
                    var now = DateTime.Now;

                    // Jointure des types de rendez-vous
                    foreach (var appointment in appointments)
                    {
                        var appointmentType = AppointmentTypes.FirstOrDefault(t => t.Id == appointment.AppointmentTypeId);
                        if (appointmentType != null)
                        {
                            appointment.AppointmentTypeLibelle = appointmentType.Libelle;
                        }
                    }

                    PastAppointments = appointments.Where(a => a.EndTime < now).OrderBy(a => a.StartTime).ToList();
                    FutureAppointments = appointments.Where(a => a.StartTime >= now).OrderBy(a => a.StartTime).ToList();
                    Console.WriteLine("Appointments loaded and sorted successfully.");
                }
                else
                {
                    Console.WriteLine("No appointments received.");
                }
                this.StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des événements : {ex.Message}");
            }
        }
    }
}
