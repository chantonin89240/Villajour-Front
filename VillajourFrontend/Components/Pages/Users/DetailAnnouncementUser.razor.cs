using Microsoft.AspNetCore.Components;
using Radzen;
using VillajourFrontend.Dto.Announcement;

namespace VillajourFrontend.Components.Pages.Users;

public partial class DetailAnnouncementUser : ComponentBase
{
    [Inject]
    protected HttpClient? HttpClient { get; set; }

    [Inject]
    protected DialogService? DialogService { get; set; }

    [Inject]
    protected NavigationManager? NavigationManager { get; set; }

    [Inject]
    private NotificationService? NotificationService { get; set; }

    [Parameter]
    public AnnouncementDto? Announcement { get; set; }
}