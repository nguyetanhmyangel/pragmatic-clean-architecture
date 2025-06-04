namespace Bookify.Application.Abstractions.Email;

public interface IEmailService
{
    Task SendAsync(ShareKernel.ValueObjects.Email recipient, string subject, string body);
}