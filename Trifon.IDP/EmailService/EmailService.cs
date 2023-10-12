using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using Trifon.IDP.EmailService.Dtos;

namespace Trifon.IDP.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendEmail(EmailDto request)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(request.FromWhom, request.From));
            email.To.Add(new MailboxAddress(request.ToWhom, request.To));
            email.Subject = request.Subject;
            email.Body = new TextPart(TextFormat.Html) { Text = request.Body };

            var emailPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD"); // Create environment variable with your email password
            using var smtp = new SmtpClient();
            smtp.Connect(_config.GetSection("EmailHost").Value, 465, true);
            smtp.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            smtp.Authenticate(_config.GetSection("EmailAddress").Value, emailPassword);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
