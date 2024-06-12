using Microsoft.AspNetCore.Mvc;
using VillajourFrontend.Entity;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Radzen;
using System.Net.Http;
using System.Net.Http.Json;
using VillajourFrontend.Components.Pages.DetailMairieUsers;

namespace VillajourFrontend.Components.Pages.Mairie
{
    public partial class AnnouncementMairie : ComponentBase
    {
        [Inject]
        protected HttpClient HttpClient { get; set; }
        [Inject]
        protected DialogService DialogService { get; set; }

        private List<Announcement> models;
        private bool isMairie = true;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                models = await HttpClient.GetFromJsonAsync<List<Announcement>>("https://664da1f7ede9a2b5565433c5.mockapi.io/api/Appointment/v1/Announcement");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load announcements: {ex.Message}");
            }
        }


        private async Task AddAnnouncement()
        {
            var newAnnouncement = new Announcement(); 
            var parameters = new Dictionary<string, object>();

           //var dialogOptions = new DialogOptions() { Width = "700px", Height = "400px" };

            var result = await DialogService.OpenAsync<AddAnnouncementMairie>("Ajouter une annonce");
            //if (result != null)
            //{
            //    HttpResponseMessage response = await HttpClient.PostAsJsonAsync("https://664da1f7ede9a2b5565433c5.mockapi.io/api/Appointment/v1/Announcement", newAnnouncement);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        models.Add(newAnnouncement);
            //        StateHasChanged();
            //    }
            //    else
            //    {
            //        Console.WriteLine("Failed to add the announcement.");
            //    }
            //}
        }





        //private async Task EditAnnouncement(int id)
        //{
        //    var announcementToEdit = models.FirstOrDefault(a => a.Id == id);
        //    if (announcementToEdit != null)
        //    {
        //        var parameters = new Dictionary<string, object>
        //{
        //    {"announcement", announcementToEdit},
        //    {"isNew", false}
        //};

        //        var result = await DialogService.OpenAsync<AnnouncementDetailMairie>("Modifier une annonce", parameters);
        //    }
        //    else
        //    {
        //        Console.WriteLine("Announcement not found.");
        //    }
        //}



        private async Task DeleteAnnouncement(int id)
        {
            HttpResponseMessage response = await HttpClient.DeleteAsync($"https://664da1f7ede9a2b5565433c5.mockapi.io/api/Appointment/v1/Announcement/{id}");
            if (response.IsSuccessStatusCode)
            {
                models = await HttpClient.GetFromJsonAsync<List<Announcement>>("https://664da1f7ede9a2b5565433c5.mockapi.io/api/Appointment/v1/Announcement/");
                StateHasChanged(); // Refresh the UI
            }
            else
            {
                // Handle errors
                Console.WriteLine("Failed to delete the announcement.");
            }
        }

        private async Task ViewDetails(int id)
        {
            var announcement = models.FirstOrDefault(a => a.Id == id);
            if (announcement != null)
            {
                var parameters = new Dictionary<string, object>
        {
            {"announcement", announcement}
        };

              await DialogService.OpenAsync<AnnouncementDetailMairie>("Détails de l'annonce", parameters);
            }
            else
            {
                Console.WriteLine("Announcement not found.");
            }
        }

    }
}
