using Microsoft.AspNetCore.Components;
using Radzen;
using VillajourFrontend.Dto.Event;

namespace VillajourFrontend.Components.Pages.Mairie;

public partial class DetailEventMairie : ComponentBase
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
    public EventDto? Event { get; set; }
}
