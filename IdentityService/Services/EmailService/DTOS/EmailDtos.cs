using System.ComponentModel.DataAnnotations;

namespace IdentityService.Services.EmailService.DTOS
{
    public record EmailResponse([Required]string Message);
    public record EmailRequest([Required]string To,
                               [Required]string Subject,
                               [Required]string Body);
}
