using Microsoft.Extensions.Options;
using PDCoreNew.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace PDCoreNew.Services.Serv
{
    public class SendGridEmalSender : IEmailSender
    {
        public SendGridEmalSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

        public Task SendEmailAsync(string email, string subject, string bodyHtml, string body = null, string name = null, string attachementPath = null, string attachementName = null)
        {
            return Execute(Options.SendGridKey, subject, body, bodyHtml, name, email, attachementPath);
        }

        public Task Execute(string apiKey, string subject, string message, string messageHtml, string name, string email, string attachementPath, string attachementName = null)
        {
            _ = attachementPath;

            _ = attachementName;

            var client = new SendGridClient(apiKey);

            var msg = new SendGridMessage()
            {
                From = new EmailAddress(Options.SendGridName, Options.SendGridUser),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = messageHtml,
            };

            msg.AddTo(new EmailAddress(email, name));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            return client.SendEmailAsync(msg);
        }
    }
}
