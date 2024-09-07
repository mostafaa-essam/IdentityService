using IdentityService.Services.EmailService.DTOS;

namespace IdentityService.Services.EmailService
{
    public interface IEmailService
    {
        Task<EmailResponse> SendEmailAsync(EmailRequest request);
    }
}
