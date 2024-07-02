using Microsoft.AspNetCore.Components;
using Radzen;
using VillajourFrontend.Entity;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using VillajourFrontend.Dto.Appointement;
using Radzen.Blazor;

namespace VillajourFrontend.Components.Pages.Mairie
{
    public partial class AppointmentMairieBase : ComponentBase
    {
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected HttpClient HttpClient { get; set; }

        protected RadzenScheduler<Appointment> scheduler;
        protected List<Appointment> appointments = new List<Appointment>();
        protected Guid UserId => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
        protected Guid MairieId => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

        protected override async Task OnInitializedAsync()
        {
            await LoadAppointments();
        }

        protected async Task LoadAppointments()
        {
            var apiUrl = $"Appointments/GetAppointmentByMairie/{MairieId}";
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
                MairieId = MairieId
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
