using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PDCoreNew.Models;
using PDCoreNew.Services.IServ;
using System;
using System.ComponentModel;
using System.Net.Mail;

namespace PDCoreNew.Services.Serv
{
    public class MailServiceAsync : MailService, IMailServiceAsync
    {
        public MailServiceAsync(IOptions<SmtpSettingsModel> smtpSettingsModel, ILogger<MailService> logger) : base(smtpSettingsModel, logger)
        {
        }

        public MailServiceAsync(ILogger<MailService> logger) : base(logger)
        {
        }

        public void SendEmailAsync(MailMessageModel mailMessageModel, SmtpSettingsModel smtpSettingsModel = null)
        {
            var data = GetData(mailMessageModel, smtpSettingsModel);

            SendEmailAsync(data.Item1, data.Item2);
        }

        public void SendEmailAsync(MailMessage message, SmtpSettingsModel smtpSettingsModel = null)
        {
            var client = GetSmtpClient(smtpSettingsModel);

            SendEmailAsync(message, client);
        }

        public void SendEmailAsync(MailMessage message)
        {
            SendEmailAsync(message, new SmtpClient());
        }

        public void SendEmailAsync(MailMessage message, SmtpClient client)
        {
            client.SendCompleted += (s, e) =>
            {
                SendCompletedCallback(s, e);

                client.Dispose();
                message.Dispose();
            };

            try
            {
                client.SendAsync(message, message);

                logger.LogInformation(SendStatusMessageFormat, "Sending async", message.To, message.Subject);
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Async email error");
            }
        }

        protected void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            var mail = (MailMessage)e.UserState;

            if (e.Error != null)
            {
                logger.LogError(e.Error, SendStatusMessageFormat, "Error sending", mail.To, mail.Subject);
            }
            else if (e.Cancelled)
            {
                logger.LogWarning(SendStatusMessageFormat, "Cancelled", mail.To, mail.Subject);
            }
            else
            {
                logger.LogInformation(SendStatusMessageFormat, "Sent email", mail.To, mail.Subject);
            }
        }
    }
}
