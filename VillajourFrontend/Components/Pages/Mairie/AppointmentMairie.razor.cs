using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using System.Security.Claims;
using VillajourFrontend.Entity;

namespace VillajourFrontend.Components.Pages.Mairie
{
    public partial class AppointmentMairieBase : ComponentBase
    {
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected HttpClient HttpClient { get; set; }

        [Inject]
        private IHttpContextAccessor? _httpContext { get; set; }
        protected Guid MairieGuid { get => new Guid(_httpContext?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value); }

        protected RadzenScheduler<Appointment> scheduler;
        protected List<Appointment> appointments = new List<Appointment>();
        protected Guid UserId => Guid.Parse("E93EE95D-2F68-4727-BD9C-E3F9F55CC8AD");

        protected override async Task OnInitializedAsync()
        {
            await LoadAppointments();
        }

        protected async Task LoadAppointments()
        {
            var apiUrl = $"Appointments/GetAppointmentByMairie/{MairieGuid}";
            try
            {
                var apiAppointments = await HttpClient.GetFromJsonAsync<List<Appointment>>(apiUrl);
                if (apiAppointments != null)
                {
                    appointments = apiAppointments;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des rendez-vous: {ex.Message}");
            }
        }

        protected async Task OnAppointmentSelect(SchedulerAppointmentSelectEventArgs<Appointment> args)
        {
            var parameters = new Dictionary<string, object> { { "CurrentAppointment", args.Data }, { "Title", "Détails du rendez-vous" } };
            var options = new DialogOptions() { Width = "700px", Height = "450px" };
            var result = await DialogService.OpenAsync<AppointmentForm>("", parameters, options);

            if (result != null)
            {
                var index = appointments.FindIndex(a => a.Id == result.Id);
                if (index != -1)
                {
                    appointments[index] = result;
                    await scheduler.Reload();
                }
            }
            else if (result == null)
            {
                appointments.Remove(args.Data);
                await scheduler.Reload();
            }
        }

        protected async Task OnSlotSelect(SchedulerSlotSelectEventArgs args)
        {
            var newAppointment = new Appointment
            {
                StartTime = args.Start,
                EndTime = args.End,
                UserId = UserId,
                MairieId = MairieGuid
            };

            var parameters = new Dictionary<string, object> { { "CurrentAppointment", newAppointment }, { "Title", "Nouveau Rendez-vous" } };
            var options = new DialogOptions() { Width = "700px", Height = "450px" };
            var result = await DialogService.OpenAsync<AppointmentForm>("", parameters, options);

            if (result != null)
            {
                appointments.Add(result);
                await scheduler.Reload();
            }
        }
    }
}
