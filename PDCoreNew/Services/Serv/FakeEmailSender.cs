using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace PDCoreNew.Services.Serv
{
    public class FakeEmailSender : IEmailSender
    {
        private readonly ILogger<FakeEmailSender> _logger;

        public FakeEmailSender(ILogger<FakeEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string bodyHtml, string body = null, string name = null, string attachementPath = null, string attachementName = null)
        {
            // Log the message
            _logger.LogInformation($"Name: {name} To: {email} Subject: {subject} Body: {body} BodyHtml: {bodyHtml} AttachementPath: {attachementPath} AttachementName: {attachementName}");

            return Task.CompletedTask;
        }
    }
}
