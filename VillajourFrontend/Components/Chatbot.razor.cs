using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;
using System.Security.Claims;
using Villajour.Domain.Entities.Chatbot;

namespace VillajourFrontend.Components;

public partial class Chatbot : ComponentBase
{
    [Inject]
    private HttpClient? _httpClient { get; set; }

    [Inject]
    private IHttpContextAccessor? _httpContext { get; set; }

    private Guid _sessionId;
    private string? _message;

    private ObservableCollection<ChatMessage> Messages { get; set; } = new ObservableCollection<ChatMessage>();

    protected async override Task OnInitializedAsync()
    {
        var userId = _httpContext?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (userId != null)
        {
            var session = await this._httpClient.GetFromJsonAsync<ChatSession>($"Chatbot/session/{userId}/getlastsession");
            _sessionId = new Guid(session?.Id);

            this.Messages = await this._httpClient.GetFromJsonAsync<ObservableCollection<ChatMessage>>($"Chatbot/{_sessionId}/messages");
        }
        
        await base.OnInitializedAsync();
    }

    private async Task SendMessage()
    {
        if (!string.IsNullOrEmpty(_message)){
            var userId = _httpContext?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId != null)
            {
                var response = await this._httpClient.PostAsJsonAsync($"Chatbot/{_sessionId}/messages", _message);

                var answer = await response.Content.ReadAsStringAsync();

                this.Messages = await this._httpClient.GetFromJsonAsync<ObservableCollection<ChatMessage>>($"Chatbot/{_sessionId}/messages");
            }
            _message = "";

            this.StateHasChanged();
        }
    }

    private async Task NewSession()
    {
        var userId = _httpContext?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId != null)
        {
            var response = await this._httpClient.PostAsJsonAsync($"Chatbot/session", new Guid(userId));

            var session = await this._httpClient.GetFromJsonAsync<ChatSession>($"Chatbot/session/{userId}/getlastsession");
            _sessionId = new Guid(session?.Id);

            this.Messages = await this._httpClient.GetFromJsonAsync<ObservableCollection<ChatMessage>>($"Chatbot/{_sessionId}/messages");
        }

        this.StateHasChanged();
    }
}
