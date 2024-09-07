using IdentityService.Services.AccountService;
using IdentityService.Services.AccountService.DTOs;
using IdentityService.Services.EmailService;
using IdentityService.Services.EmailService.DTOS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IEmailService _emailService;

        public AccountsController(IAccountService accountService, IEmailService emailService)
        {
            _accountService = accountService;
            _emailService = emailService;
        }

        [HttpPost]
        [Route("Login")]
        public async Task< IActionResult>LoginAsync([FromBody]LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            else
            { 
            var result=await _accountService.LogInAsync(request);
                if(string.IsNullOrEmpty(result.Token))
                    return BadRequest("Invalid Username or Password");
                else
                    return Ok(result);
            }
        }
        [HttpPost]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmailAsync([FromBody] ConfirmEmailRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            else
            {
                var result=await _accountService.ConfirmEmailAsync(request);
                if (result.Message.Contains("Email Confirmed"))
                    return Ok(result.Message);
                else
                    return BadRequest(result.Message);
            }
        }
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            else
            {
                var result = await _accountService.RegisterAsync(request);
                if (result.Message.Contains("Created"))
                    return Ok(result.Message);
                else
                    return BadRequest(result.Message);
            }
        }
        [HttpPost]
        [Route("ForgetPassword")]
        public async Task<IActionResult> ForgetPassowrdAsync([FromBody]ForgetPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            else
            {
                var result = await _accountService.ForgetPasswordAsync(request);
                if (result.Message.Contains("Reset"))
                    return Ok(result.Message);
                else
                    return BadRequest(result.Message);
            }
        }
        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody]ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            else
            {
                var result = await _accountService.ResetPasswordAsync(request);
                if (result.Message.Contains("Successfully"))
                    return Ok(result.Message);
                else
                    return BadRequest(result.Message);
            }
        }
    }
}
