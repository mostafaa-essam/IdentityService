using IdentityService.Models;
using IdentityService.Services.EmailService.DTOS;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net.Mail;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace IdentityService.Services.EmailService
{
    public class EmailService:IEmailService
    {
        private readonly EmailOptions _emailOptions;

        public EmailService(IOptions<EmailOptions> options)
        {
            _emailOptions = options.Value;
        }

        public Task<EmailResponse> SendEmailAsync(EmailRequest request)
        {
            // Create the message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailOptions.DisplayName, _emailOptions.Email));
            message.To.Add(new MailboxAddress(" ", request.To));
            message.Subject = request.Subject;
            
            // Create the body content
            message.Body = new TextPart("plain")
            {
                Text = request.Body
            };
            var result=string.Empty;
            // Connect and authenticate with the SMTP server
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    // Connect to the SMTP server, replace with your server details
                    client.Connect(_emailOptions.Host, _emailOptions.Port, SecureSocketOptions.StartTls);

                    // Authenticate if necessary (replace with your credentials)
                    client.Authenticate(_emailOptions.Email, _emailOptions.Password);

                    // Send the email
                    client.Send(message);
                     result="Message Sent";
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                     result = $"Error: {ex.Message}";

                }
                finally
                {
                    // Disconnect from the SMTP server
                    client.Disconnect(true);
                }
                return Task.FromResult( new EmailResponse(result));
              
            }
        }


    }
    
}
