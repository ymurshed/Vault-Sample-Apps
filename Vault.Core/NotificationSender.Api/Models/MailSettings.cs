namespace NotificationSender.Api.Models
{
    public class MailSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Sender { get; set; }
        public string Password { get; set; }
    }
}
