using System.ComponentModel.DataAnnotations;

namespace NotificationSender.Api.Models
{
    public class NotificationModel
    {
        [Required]
        [EmailAddress]
        public string Recipient { get; set; }
        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }
    }
}
