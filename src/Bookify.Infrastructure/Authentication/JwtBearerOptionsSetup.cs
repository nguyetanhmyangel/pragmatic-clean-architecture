using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Bookify.Infrastructure.Authentication;
public class JwtBearerOptionsSetup(IOptions<AuthenticationOptions> options) : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly AuthenticationOptions _options = options.Value;

    public void Configure(string name, JwtBearerOptions options) => Configure(options);

    public void Configure(JwtBearerOptions options)
    {
        options.Audience = _options.Audience;
        options.MetadataAddress = _options.MetadataUrl;
        options.RequireHttpsMetadata = _options.RequireHttpsMetadata;
        options.TokenValidationParameters.ValidIssuer = _options.ValidIssuer;
    }
}
/*
    - Audience: Xác định "đối tượng" (aud) trong token JWT. Thường dùng để kiểm tra token đó có 
        hợp lệ cho API nào. Trong Keycloak mặc định có một client tên là account.
    - ValidIssuer: Địa chỉ (Issuer) của realm trong Keycloak. Token nhận được phải chứa iss 
        bằng chuỗi này mới được coi là hợp lệ. Định dạng: http://{host}/realms/{realm-name}/
    - MetadataUrl: URL chứa cấu hình OpenID Connect của realm (tức là nơi chứa public keys, 
        endpoints để xác thực token, lấy token, v.v.). Keycloak cung cấp endpoint này tự động.
    - RequireHttpsMetadata: Nếu đặt là true, ứng dụng chỉ chấp nhận MetadataUrl thông qua HTTPS. 
        Trong môi trường dev (localhost), có thể tạm để false.
    - TokenValidationParameters: Một đối tượng chứa các tham số để xác thực token JWT.
    - options.TokenValidationParameters.ValidIssuer: có vai trò xác định giá trị iss (issuer) 
        trong token JWT phải khớp với giá trị  cấu hình, để đảm bảo token đó thực sự đến từ 
        Keycloak realm chỉ định. ValidIssuer giúp xác nhận token không bị giả mạo, đảm bảo 
        nó đến từ đúng realm Keycloak cho phép, tránh việc token từ một nơi khác bị dùng sai mục đích.
 */
