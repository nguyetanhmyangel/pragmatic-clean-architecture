using Bookify.Application.Abstractions.Email;

namespace Bookify.Infrastructure.Services;
internal sealed class EmailService : IEmailService
{
    public Task SendAsync(ShareKernel.ValueObjects.Email recipient, string subject, string body)
    {
        return Task.CompletedTask;
    }
}
