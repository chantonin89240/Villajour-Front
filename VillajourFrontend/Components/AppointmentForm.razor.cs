using Microsoft.AspNetCore.Components;
using Radzen;
using VillajourFrontend.Entity;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace VillajourFrontend.Components
{
    public partial class AppointmentFormBase : ComponentBase
    {
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected HttpClient HttpClient { get; set; }

        [Parameter]
        public VillajourFrontend.Entity.Appointment CurrentAppointment { get; set; }

        [Parameter]
        public DateTime Start { get; set; }

        [Parameter]
        public DateTime End { get; set; }

        protected VillajourFrontend.Entity.Appointment model = new VillajourFrontend.Entity.Appointment();

        protected override void OnParametersSet()
        {
            if (CurrentAppointment != null)
            {
                model = CurrentAppointment;
            }
            else
            {
                model.DateStart = Start;
                model.DateEnd = End;
            }
        }

        protected void OnSubmit()
        {
            if (!string.IsNullOrEmpty(model.Title))
            {
                DialogService.Close(model);
            }
        }

        protected async Task ValidateAppointment()
        {
            model.Validation = true;
            DialogService.Close(model);
        }

        protected async Task RefuseAppointment()
        {
            if (model.Id != "0")
            {
                var apiUrl = $"https://664da1f7ede9a2b5565433c5.mockapi.io/api/Appointment/v1/Appointment/{model.Id}";
                var response = await HttpClient.DeleteAsync(apiUrl);
                response.EnsureSuccessStatusCode();
            }
            DialogService.Close(null);
        }

        protected async Task CancelAppointment()
        {
            model.Validation = false;
            DialogService.Close(model);
        }
    }
}
