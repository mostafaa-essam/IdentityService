using IdentityService.Models;
using IdentityService.Services.AccountService.DTOs;
using IdentityService.Services.EmailService;
using IdentityService.Services.EmailService.DTOS;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IdentityService.Services.AccountService
{
    public class AccountService:IAccountService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;

        public AccountService(IOptions<JwtOptions> jwtOptions, UserManager<User> userManager, IEmailService emailService)
        {
            _jwtOptions = jwtOptions.Value;
            _userManager = userManager;
            _emailService = emailService;
        }
        
        public async Task<LoginResponse> LogInAsync(LoginRequest request)
        
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user is not null 
                && await _userManager.CheckPasswordAsync(user, request.Password)
                )
            {
                var token = CreateToken(user);
                var refreshToken = CreateRefreshToken();
                return new LoginResponse(token, refreshToken.Token, refreshToken.ExpiresOn);
            }

            else

                return new LoginResponse(string.Empty, null, null);

        }
        private string CreateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescripotor = new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey)), SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.Name,user.FullName),
                    new(ClaimTypes.MobilePhone,user.PhoneNumber!),
                    

                }
                )
            };
            var securityToken = tokenHandler.CreateToken(tokenDescripotor);
            var accessToken = tokenHandler.WriteToken(securityToken);
            return accessToken;
        }
        private RefreshToken CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var generator = new RNGCryptoServiceProvider(randomNumber);
            generator.GetBytes(randomNumber);

            return new RefreshToken()
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddMinutes(30),
                CreatedOn = DateTime.UtcNow
            };

        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest input)
        {
            //Email Token:-
            var user = new User { 
                FullName=input.FullName,
                PhoneNumber=input.PhoneNumber,
                UserName=input.UserName,
                Email=input.Email,
                EmailConfirmed=false,
                Id=Guid.NewGuid().ToString(),
                TwoFactorEnabled=false,
                PhoneNumberConfirmed=false,
            };
            var result= await _userManager.CreateAsync(user,input.Password);
          
            if (result.Succeeded)
            {  
                //Create Confirmation Token:-
                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //Send Confirmation Token
                await _emailService.SendEmailAsync(new EmailRequest(input.Email, "Email Confirmation", emailConfirmationToken));
                return new RegisterResponse("User Created Successfully");
            }
            else
                return new RegisterResponse("Something Went Wrong");



        }

        public async Task<ConfirmEmailResponse> ConfirmEmailAsync(ConfirmEmailRequest input)
        {
            if (input.Token == null || input.Email == null)
                return new ConfirmEmailResponse(" Email And Token Are Required");
            

            var user = await _userManager.FindByEmailAsync(input.Email);

            if (user == null)  
                return new ConfirmEmailResponse("Invalid Email");

            

            var result = await _userManager.ConfirmEmailAsync(user, input.Token);

            if (result.Succeeded)
                return new ConfirmEmailResponse("Email Confirmed");
            else
                return new ConfirmEmailResponse("Something Went Wrong");


        }

        public async Task<ForgetPasswordResponse> ForgetPasswordAsync(ForgetPasswordRequest input)
        {
            if (string.IsNullOrEmpty(input.Email))
                return new ForgetPasswordResponse("Email Is Required");
            var user=await _userManager.FindByEmailAsync(input.Email);
            if(user is null)
                return new ForgetPasswordResponse("Invalid Email");
            //Generate Token:-
            var forgotPassowrodToken=await _userManager.GeneratePasswordResetTokenAsync(user);
            //Send Token:-
            await _emailService.SendEmailAsync(new EmailRequest(input.Email, "Your Forgot Password Token Is ", forgotPassowrodToken));
            return new ForgetPasswordResponse("Check your Email for Reset Token");


        }


        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest input)
        {
            if (input is null)
                return new ResetPasswordResponse("All Fields Are Required");
            if(string.IsNullOrEmpty(input.Password)&& string.IsNullOrEmpty(input.ConfirmPassword))
                return new ResetPasswordResponse("Password field can't be null or Empty string");
            var user=await _userManager.FindByEmailAsync(input.Email);
            if(user is null)
                return new ResetPasswordResponse("Invalid Email");
            if(!input.Password.Equals(input.ConfirmPassword))
                return new ResetPasswordResponse("Both Password Should be match");
            var result = await _userManager.ResetPasswordAsync(user, input.Token, input.Password);
            if(result.Succeeded)
                return new ResetPasswordResponse("Password Reset Successfully");
            else
                return new ResetPasswordResponse("Something Went Wrong");

        }
    }
}
