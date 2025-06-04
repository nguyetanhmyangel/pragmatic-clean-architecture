using Bookify.Application.Abstractions.Authentication;
using Bookify.Domain.Entities.Users;
using Bookify.Infrastructure.Authentication.Models;
using System.Net.Http.Json;
using Bookify.ShareKernel.Errors;
using Bookify.ShareKernel.Utilities;
using Microsoft.Extensions.Options;

namespace Bookify.Infrastructure.Authentication;


/// <summary>
/// * AuthenticationService dùng xác thực dùng HttpClient để giao tiếp với Keycloak,
///   giúp thực hiện thao tác đăng ký người dùng (user registration) thông qua API của Keycloak.
/// </summary>
internal sealed class AuthenticationService(HttpClient httpClient, IOptions<KeycloakOptions> keycloakOptions) : IAuthenticationService
{
    private const string PasswordCredentialType = "password";
    // public async Task<string> RegisterAsync(User user, string password, CancellationToken cancellationToken = default)
    // {
    //     //chuyển đổi dữ liệu từ lớp User sang UserRepresentationModel — định dạng mà Keycloak hiểu.
           //var userRepresentationModel = UserRepresentationModel.FromUser(user);
    //
    //     userRepresentationModel.Credentials =
    //     [
    //         new()
    //         {
    //             Value = password,
    //             Temporary = false,
    //             Type = PasswordCredentialType
    //         }
    //     ];
    //
    //     var response = await httpClient.PostAsJsonAsync(
    //         "users",
    //         userRepresentationModel,
    //         cancellationToken);
    //
    //     if (response.StatusCode == HttpStatusCode.Conflict)
    //     {
    //         throw new InvalidOperationException("Người dùng đã tồn tại.");
    //     }
    //
    //     if (!response.IsSuccessStatusCode)
    //     {
    //         var error = await response.Content.ReadAsStringAsync(cancellationToken);
    //         throw new InvalidOperationException($"Lỗi khi tạo người dùng: {response.StatusCode} - {error}");
    //     }
    //
    //     return ExtractIdentityIdFromLocationHeader(response);
    // }

    private readonly KeycloakOptions _options = keycloakOptions.Value;
    public async Task<Result<string>> RegisterAsync(User user, string password, CancellationToken cancellationToken)
    {
        var userRepresentation = UserRepresentationModel.FromUser(user);
        userRepresentation.Credentials = new[]
        {
            new CredentialRepresentationModel
            {
                Type = "password",
                Value = password,
                Temporary = false
            }
        };

        var path = $"{_options.AdminEndpoint}/users";
        var response = await httpClient.PostAsJsonAsync(path, userRepresentation, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var userId = ExtractIdentityIdFromLocationHeader(response)
                         ?? await GetUserIdByEmail(user.Email.Value, cancellationToken);

            return userId is not null
                ? Result.Success(userId)
                : Result.Failure<string>(Error.NullValue);
        }

        if ((int)response.StatusCode == 409)
        {
            var userId = await GetUserIdByEmail(user.Email.Value, cancellationToken);
            return userId is not null
                ? Result.Success(userId)
                : Result.Failure<string>(new Error("Keycloak.UserExistsButCannotResolve", "User exists but could not resolve ID"));
        }

        return Result.Failure<string>(new Error("Keycloak.RegisterFailed", $"Keycloak response: {response.StatusCode}"));
    }

    //lấy identityId nếu user đã tồn tại ở Keycloak
    private async Task<string?> GetUserIdByEmail(string email, CancellationToken cancellationToken)
    {
        var path = $"{_options.AdminEndpoint}/users?email={email}";
        var response = await httpClient.GetAsync(path, cancellationToken);

        if (!response.IsSuccessStatusCode)
            return null;

        var users = await response.Content.ReadFromJsonAsync<List<KeycloakUserDto>>(cancellationToken: cancellationToken);
        return users?.FirstOrDefault()?.Id;
    }

    // Keycloak sau khi tạo user mới sẽ trả về header Location chứa URL như:
    // Location: https://keycloak-server/admin/realms/myrealm/users/12345-abcde
    // Kiểm tra xem header Location có tồn tại không. Nếu có,
    // Cắt chuỗi để lấy phần ID cuối cùng (12345-abcde) làm userId.
    private static string? ExtractIdentityIdFromLocationHeader(HttpResponseMessage response)
    {
        const string usersSegmentName = "users/";
        var locationHeader = response.Headers.Location?.ToString();

        if (locationHeader is null) return null;

        var userSegmentIndex = locationHeader.IndexOf(usersSegmentName, StringComparison.InvariantCultureIgnoreCase);
        return userSegmentIndex >= 0
            ? locationHeader[(userSegmentIndex + usersSegmentName.Length)..]
            : null;
    }
}
