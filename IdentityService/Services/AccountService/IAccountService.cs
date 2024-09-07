using IdentityService.Services.AccountService.DTOs;

namespace IdentityService.Services.AccountService
{
    public interface IAccountService
    {
        public Task<LoginResponse> LogInAsync(LoginRequest input);
        public Task<RegisterResponse> RegisterAsync(RegisterRequest input);
        public Task<ConfirmEmailResponse> ConfirmEmailAsync(ConfirmEmailRequest input);
        public Task<ForgetPasswordResponse> ForgetPasswordAsync(ForgetPasswordRequest input);
        public Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest input);
    }
}
