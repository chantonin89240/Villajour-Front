using Radzen;
using System.Reflection;
using VillajourFrontend.Components.Pages.DetailMairieUsers;
using VillajourFrontend.Entity;

namespace VillajourFrontend.Components.Pages.Users
{
    public partial class AnnouncementUser
    {
        private List<Announcement> userFavorites = new List<Announcement>();
        private List<Announcement> mairieAnnouncements = new List<Announcement>();
        private Guid currentUserId = new Guid();
        protected override async Task OnInitializedAsync()
        {
            await LoadUserFavorites();
            await LoadMairieAnnouncements();
        }

        private async Task LoadUserFavorites()
        {
            var favorites = await HttpClient.GetFromJsonAsync<List<FavoriteContent>>($"https://664da1f7ede9a2b5565433c5.mockapi.io/api/Appointment/v1/Announcement");
            var announcementIds = favorites.Where(f => f.AnnouncementId != null).Select(f => f.AnnouncementId.Value).ToList();
            userFavorites = await HttpClient.GetFromJsonAsync<List<Announcement>>($"https://664da1f7ede9a2b5565433c5.mockapi.io/api/Appointment/v1/Announcement/{string.Join(",", announcementIds)}");
        }

        private async Task LoadMairieAnnouncements()
        {
        }

        private async Task ViewDetails(int id)
        {
           
        }
    }
}
