namespace Bookify.Infrastructure.Authentication;
public sealed class KeycloakOptions
{
    public string AdminUrl { get; set; } = string.Empty;
    public string TokenUrl { get; set; } = string.Empty;
    public string AdminClientId { get; init; } = string.Empty;
    public string AdminClientSecret { get; init; } = string.Empty;
    public string AuthClientId { get; init; } = string.Empty;
    public string AuthClientSecret { get; init; } = string.Empty;
}
/*
    - AdminUrl: Địa chỉ của trang quản trị realm bookify.
    - TokenUrl: Endpoint để nhận token (access token, refresh token) thông qua client credentials 
        hoặc password grant. Đây là endpoint tiêu chuẩn theo OpenID Connect.
    - AdminClientId / AdminClientSecret: Thông tin của client có quyền quản trị trong realm bookify. 
        Được dùng để tương tác với Keycloak Admin API (ví dụ: tạo user, cập nhật roles...).
    - AuthClientId / AuthClientSecret: Thông tin của client được dùng cho mục đích xác thực 
        người dùng cuối (end user authentication), ví dụ: đăng nhập thông qua ứng dụng frontend.
 */
