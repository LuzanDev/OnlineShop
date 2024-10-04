using SendGrid.Helpers.Mail;
using SendGrid;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace OnlineShop.Models.Email
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public SendGridEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var apiKey = _configuration["SendGrid:ApiKey"];
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress("notification.onlineshop@gmail.com", "OnlineShop");
            var to = new EmailAddress(email);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
