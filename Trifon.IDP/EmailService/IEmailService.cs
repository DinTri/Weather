using Trifon.IDP.EmailService.Dtos;

namespace Trifon.IDP.EmailService
{
    public interface IEmailService
    {
        void SendEmail(EmailDto request);
    }
}
