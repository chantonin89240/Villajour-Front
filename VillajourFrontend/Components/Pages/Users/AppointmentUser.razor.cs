using Microsoft.AspNetCore.Components;
using Radzen;
using System.Security.Claims;
using VillajourFrontend.Dto.Appointement;

namespace VillajourFrontend.Components.Pages.Users
{
    public partial class AppointmentUserBase : ComponentBase
    {
        [Inject]
        protected HttpClient? HttpClient { get; set; }

        [Inject]
        protected DialogService? DialogService { get; set; }

        [Inject]
        private IHttpContextAccessor? _httpContext { get; set; }
        protected Guid userGuid { get => new Guid(_httpContext?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value); }

        protected List<AppointmentDTO> PastAppointments { get; set; } = new List<AppointmentDTO>();
        protected List<AppointmentDTO> FutureAppointments { get; set; } = new List<AppointmentDTO>();
        protected List<AppointmentTypeDTO> AppointmentTypes { get; set; } = new List<AppointmentTypeDTO>();

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
