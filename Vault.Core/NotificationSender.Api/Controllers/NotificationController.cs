using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NotificationSender.Api.Models;
using NotificationSender.Api.Services;

namespace NotificationSender.Api.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;
        
        public NotificationController(IMailService mailService, IConfiguration configuration)
        {
            _mailService = mailService;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok($"{_configuration.GetSection("AppName").Value} is running...");
        }

        [HttpPost]
        public IActionResult SendNotification([FromBody] NotificationModel model)
        {
            var message = _mailService.CreateMailMessage
                            (
                                model.Recipient,
                                model.Subject, 
                                model.Body
                            );

            var client = _mailService.GetSmtpClient();
            client.Send(message);
            return Ok();
        }
    }
}
