using Bookify.Infrastructure.Authentication.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Bookify.Infrastructure.Authentication;

/// <summary>
/// HTTP delegating handler để tự động thêm JWT token vào các request gửi tới
/// Keycloak khi thực hiện các lệnh quản trị (admin)
/// </summary>
/// <param name="keycloakOptions"></param>
public sealed class AdminAuthorizationDelegatingHandler(IOptions<KeycloakOptions> keycloakOptions) : DelegatingHandler
{
    private readonly KeycloakOptions _keycloakOptions = keycloakOptions.Value;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // lấy access token.
        var authorizationToken = await GetAuthorizationToken(cancellationToken);
        //Gắn token vào header:
        request.Headers.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme, authorizationToken.AccessToken);
        //Gửi request đến server (qua base handler):
        var httpResponseMessage = await base.SendAsync(request, cancellationToken);
        //Gọi EnsureSuccessStatusCode() để đảm bảo request thành công (status code 2xx). Nếu không, nó ném HttpRequestException.
        httpResponseMessage.EnsureSuccessStatusCode();
        return httpResponseMessage;
    }

    //Gửi request tới endpoint của Keycloak để lấy access token bằng Client Credentials Flow:
    private async Task<AuthorizationToken> GetAuthorizationToken(CancellationToken cancellationToken)
    {
        var path = _keycloakOptions.TokenEndpoint.TrimStart('/');

        var request = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", _keycloakOptions.AdminClientId),
                new KeyValuePair<string, string>("client_secret", _keycloakOptions.AdminClientSecret),
                new KeyValuePair<string, string>("scope", "openid email"),
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            })
        };

        var response = await base.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<AuthorizationToken>(cancellationToken: cancellationToken)
               ?? throw new ApplicationException("Authorization token could not be deserialized.");
    }


}
