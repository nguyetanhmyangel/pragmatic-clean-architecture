namespace Bookify.Infrastructure.Authentication;
public class AuthenticationOptions
{
    public string Audience { get; init; } = string.Empty;
    public string Realm { get; init; } = string.Empty;
    public string AuthorityBaseUrl { get; init; } = string.Empty;
    public bool RequireHttpsMetadata { get; init; }

    public string MetadataUrl =>
        $"{AuthorityBaseUrl.TrimEnd('/')}/realms/{Realm}/.well-known/openid-configuration";

    public string ValidIssuer =>
        $"{AuthorityBaseUrl.TrimEnd('/')}/realms/{Realm}";
}
