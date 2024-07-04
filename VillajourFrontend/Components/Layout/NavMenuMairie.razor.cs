using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Radzen;
using System.Diagnostics;
using System.Security.Claims;
using VillajourFrontend.Dto.Announcement;

namespace VillajourFrontend.Components.Layout;

public partial class NavMenuMairie
{
    [Inject]
    protected HttpClient? HttpClient { get; set; }

    [Inject]
    protected DialogService? DialogService { get; set; }

    [Inject]
    protected NavigationManager? NavigationManager { get; set; }

    [Inject]
    private NotificationService? NotificationService { get; set; }

    [Inject]
    private IHttpContextAccessor? _httpContext { get; set; }

    protected override async Task OnInitializedAsync()
    {
    }

    protected async Task LogoutMairie()
    {
        _httpContext.HttpContext.SignOutAsync();

        this.NavigationManager.NavigateTo("/");
    }
}
