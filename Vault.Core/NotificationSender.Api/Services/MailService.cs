using System.Net.Mail;
using Microsoft.Extensions.Options;
using NotificationSender.Api.Models;

namespace NotificationSender.Api.Services
{
    public class MailService : IMailService
    {
        private readonly IOptions<MailSettings> _mailSettings;

        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings;
        }

        public MailMessage CreateMailMessage(string recipient, string subject, string body)
        {
            var mail = new MailMessage {From = new MailAddress(_mailSettings.Value.Sender)};
            mail.To.Add(recipient);
            mail.Subject = subject;
            mail.Body = body;
            return mail;
        }

        public SmtpClient GetSmtpClient()
        {
            var smtpClient = new SmtpClient(_mailSettings.Value.Host)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Port = _mailSettings.Value.Port,
                Credentials = new System.Net.NetworkCredential(_mailSettings.Value.Sender, _mailSettings.Value.Password)
            };
            return smtpClient;
        }
    }
}