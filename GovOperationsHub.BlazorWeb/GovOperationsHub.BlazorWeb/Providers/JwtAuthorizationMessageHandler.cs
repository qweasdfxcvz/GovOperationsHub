using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Authorization;

namespace GovOperationsHub.BlazorWeb.Providers;

public class JwtAuthorizationMessageHandler : DelegatingHandler
{
    private readonly AuthenticationStateProvider _authStateProvider;

    public JwtAuthorizationMessageHandler(AuthenticationStateProvider authStateProvider)
    {
        _authStateProvider = authStateProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        // إذا كان مسجل دخول، اسحب التوكن المخفي وأرسله للـ API
        if (user.Identity != null && user.Identity.IsAuthenticated)
        {
            var token = user.FindFirst("RawToken")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}