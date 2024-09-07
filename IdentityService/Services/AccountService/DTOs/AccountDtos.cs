using System.ComponentModel.DataAnnotations;

namespace IdentityService.Services.AccountService.DTOs
{
    public record LoginRequest([Required]string UserName,[Required] string Password);
    public record LoginResponse(string Token, string? RefreshToken, DateTime? RefreshTokenExpiration);
    public record RegisterResponse(string Message);
    public record RegisterRequest([Required]string FullName,
                                  [Required]string UserName,
                                  [Required][EmailAddress]string Email,
                                  [Required]string Password,
                                  [Required]string PhoneNumber)
                                  ;
    public record ConfirmEmailRequest([Required][EmailAddress]string Email,[Required]string Token);
    public record ConfirmEmailResponse(string Message);
    public record ForgetPasswordRequest([Required][EmailAddress]string Email);
    public record ForgetPasswordResponse(string Message);
    public record ResetPasswordRequest([Required][EmailAddress]string Email,
                                       [Required]string Token,
                                       [Required]string Password,
                                       [Required]string ConfirmPassword);
    public record ResetPasswordResponse(string Message);



}
