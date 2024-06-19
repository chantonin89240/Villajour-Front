using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace VillajourFrontend.Helper;

public class JwtTokenHeaderHandler : DelegatingHandler
{
    private IHttpContextAccessor httpContextAccessor;

    public JwtTokenHeaderHandler(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var httpContext = httpContextAccessor.HttpContext ??
        throw new InvalidOperationException(
            "No HttpContext available from the IHttpContextAccessor!");

        var accessToken = await httpContext.GetTokenAsync("access_token");

        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                accessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
