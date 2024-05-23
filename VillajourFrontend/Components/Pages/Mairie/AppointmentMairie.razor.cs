using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using System.Net.Http;
using System.Net.Http.Json;
using VillajourFrontend.Entity;

namespace VillajourFrontend.Components.Pages.Mairie
{
    public partial class AppointmentMairieBase : ComponentBase
    {
        [Inject]
        protected HttpClient HttpClient { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        protected RadzenScheduler<VillajourFrontend.Entity.Appointment> scheduler;
        protected List<Appointment> appointments = new List<Appointment>();
        protected int UserId => 1;
        protected int MairieId => 34;
        protected bool IsMairie => true;

        protected override async Task OnInitializedAsync()
        {
            await LoadAppointments();
        }

        protected async Task LoadAppointments()
        {
            var apiUrl = "https://664da1f7ede9a2b5565433c5.mockapi.io/api/Appointment/v1/Appointment";
            try
            {
                var apiAppointments = await HttpClient.GetFromJsonAsync<List<Appointment>>(apiUrl);
                appointments = apiAppointments?.ToList() ?? new List<VillajourFrontend.Entity.Appointment>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des rendez-vous: {ex.Message}");
            }
        }

        protected async Task SubmitAppointment(VillajourFrontend.Entity.Appointment model)
        {
            var apiUrl = "https://664da1f7ede9a2b5565433c5.mockapi.io/api/Appointment/v1/Appointment";
            var apiModel = new Appointment
            {
                Id = model.Id,
                UserID = model.UserID,
                MairieId = model.MairieId,
                DateStart = model.DateStart,
                DateEnd = model.DateEnd,
                Title = model.Title,
                Description = model.Description,
                Validation = model.Validation,
                AppointmentTypeId = model.AppointmentTypeId
            };

            try
            {
                HttpResponseMessage response;
                if (model.Id == null)
                {
                    response = await HttpClient.PostAsJsonAsync(apiUrl, apiModel);
                }
                else
                {
                    response = await HttpClient.PutAsJsonAsync($"{apiUrl}/{model.Id}", apiModel);
                }
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'envoi de rendez-vous: {ex.Message}");
            }
        }

        protected void OnSlotRender(SchedulerSlotRenderEventArgs args)
        {
            if (args.Start < DateTime.Now)
            {
                args.Attributes["style"] = "background-color: #d3d3d3; color: #a9a9a9;";
            }

            // var start = TimeOnly.FromDateTime(args.Start);
            // var end = TimeOnly.FromDateTime(args.End);
            // bool isValidSlot = (start >= morningStart && start < morningEnd) || (start >= afternoonStart && start < afternoonEnd);

            // if (!isValidSlot && args.View.Text != "Month")
            // {
            //     args.Attributes["style"] = "background-color: #d3d3d3; color: #a9a9a9;";
            // }
        }

        protected async Task OnSlotSelect(SchedulerSlotSelectEventArgs args)
        {
            // var start = TimeOnly.FromDateTime(args.Start);
            // var end = TimeOnly.FromDateTime(args.End);
            // bool isValidSlot = (start >= morningStart && end <= morningEnd) || (start >= afternoonStart && end <= afternoonEnd);

            // if (args.Start >= DateTime.Now && args.View.Text != "Year" && (isValidSlot || args.View.Text == "Month"))
            if (args.Start >= DateTime.Now && args.View.Text != "Year")
            {
                var parameters = new Dictionary<string, object>
                {
                    { "Start", args.Start },
                    { "End", args.End }
                };
                var data = await DialogService.OpenAsync<AppointmentForm>("Prendre un rendez-vous", parameters);

                if (data != null)
                {
                    data.UserID = UserId;
                    data.MairieId = MairieId;
                    data.Validation = false;

                    appointments.Add(data);
                    await SubmitAppointment(data);
                    await scheduler.Reload();
                }
            }
        }

        protected async Task OnAppointmentSelect(SchedulerAppointmentSelectEventArgs<VillajourFrontend.Entity.Appointment> args)
        {
            var parameters = new Dictionary<string, object> { { "CurrentAppointment", args.Data } };
            var result = await DialogService.OpenAsync<AppointmentForm>("Détails du rendez-vous", parameters);

            if (result != null)
            {
                if (result.Validation)
                {
                    var index = appointments.FindIndex(a => a.Id == result.Id);
                    if (index != -1)
                    {
                        appointments[index] = result;
                        await SubmitAppointment(result);
                        await scheduler.Reload();
                    }
                }
                else if (result == null)
                {
                    appointments.Remove(args.Data);
                    await scheduler.Reload();
                }
            }
        }

        protected void OnAppointmentRender(SchedulerAppointmentRenderEventArgs<VillajourFrontend.Entity.Appointment> args)
        {
            if (!args.Data.Validation)
            {
                args.Attributes["style"] = "background: red";
                args.Attributes["title"] = "Non validé";
            }
            else
            {
                args.Attributes["style"] = "background: green";
                args.Attributes["title"] = "Validé";
            }
        }

        protected async Task OnAppointmentMove(SchedulerAppointmentMoveEventArgs args)
        {
            var draggedAppointment = appointments.FirstOrDefault(x => x == args.Appointment.Data);
            if (draggedAppointment != null)
            {
                draggedAppointment.DateStart += args.TimeSpan;
                draggedAppointment.DateEnd += args.TimeSpan;
                await SubmitAppointment(draggedAppointment);
                await scheduler.Reload();
            }
        }

        protected async Task AdjustSlots()
        {
            var parameters = new Dictionary<string, object>
            {
                // { "MorningStart", morningStart },
                // { "MorningEnd", morningEnd },
                // { "AfternoonStart", afternoonStart },
                // { "AfternoonEnd", afternoonEnd }
            };
            var result = await DialogService.OpenAsync<SlotAdjustmentForm>("Ajuster les plages d'horaires", parameters);

            if (result != null)
            {
                // morningStart = result.MorningStart;
                // morningEnd = result.MorningEnd;
                // afternoonStart = result.AfternoonStart;
                // afternoonEnd = result.AfternoonEnd;
                await scheduler.Reload();
            }
        }

        protected async Task ValidateAppointment(VillajourFrontend.Entity.Appointment appointment)
        {
            appointment.Validation = true;
            await SubmitAppointment(appointment);
            await scheduler.Reload();
        }
    }
}
