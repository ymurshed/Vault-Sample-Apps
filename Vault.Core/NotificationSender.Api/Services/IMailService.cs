using System.Net.Mail;

namespace NotificationSender.Api.Services
{
    public interface IMailService
    {
        SmtpClient GetSmtpClient();
        MailMessage CreateMailMessage(string recipient, string subject, string body);
    }
}
