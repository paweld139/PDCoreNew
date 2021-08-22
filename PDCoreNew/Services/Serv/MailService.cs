using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PDCoreNew.Models;
using PDCoreNew.Services.IServ;
using System;
using System.Configuration;
using System.Net.Mail;

namespace PDCoreNew.Services.Serv
{
    public class MailService : IMailService
    {
        protected const string SendStatusMessageFormat = "{0} email to {1} with subject [{2}]";

        private readonly SmtpSettingsModel smtpSettingsModel;

        public MailService(IOptions<SmtpSettingsModel> smtpSettingsModel, ILogger<MailService> logger) : this(logger)
        {
            this.smtpSettingsModel = smtpSettingsModel.Value;
        }

        protected readonly ILogger<MailService> logger;

        public MailService(ILogger<MailService> logger)
        {
            this.logger = logger;
        }

        private void PrepareSending(ref SmtpSettingsModel smtpSettingsModel)
        {
            if (this.smtpSettingsModel != null) // Zostało przekazane proprzez konstruktor
            {
                smtpSettingsModel = this.smtpSettingsModel;
            }
            else if (smtpSettingsModel == null) // Nie zostało przekazane proprzez konstruktor i poprzez metodę
            {
                var appSettings = ConfigurationManager.AppSettings;

                smtpSettingsModel = new SmtpSettingsModel(appSettings);
            }
        }

        protected Tuple<MailMessage, SmtpClient> GetData(MailMessageModel mailMessageModel, SmtpSettingsModel smtpSettingsModel)
        {
            PrepareSending(ref smtpSettingsModel);

            var message = mailMessageModel.GetMailMessage(smtpSettingsModel);

            var client = smtpSettingsModel.GetSmtpClient();

            return Tuple.Create(message, client);
        }

        protected SmtpClient GetSmtpClient(SmtpSettingsModel smtpSettingsModel)
        {
            PrepareSending(ref smtpSettingsModel);

            return smtpSettingsModel.GetSmtpClient();
        }

        public void SendEmail(MailMessageModel mailMessageModel, SmtpSettingsModel smtpSettingsModel = null)
        {
            var data = GetData(mailMessageModel, smtpSettingsModel);

            SendEmail(data.Item1, data.Item2);
        }

        public void SendEmail(MailMessage message, SmtpSettingsModel smtpSettingsModel = null)
        {
            var client = GetSmtpClient(smtpSettingsModel);

            SendEmail(message, client);
        }

        public void SendEmail(MailMessage message)
        {
            SendEmail(message, new SmtpClient());
        }

        public void SendEmail(MailMessage message, SmtpClient client)
        {
            try
            {
                logger.LogInformation(SendStatusMessageFormat, "Sending sync", message.To, message.Subject);

                client.Send(message);

                logger.LogInformation(SendStatusMessageFormat, "Sent email", message.To, message.Subject);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, SendStatusMessageFormat, "Error sending", message.To, message.Subject);
            }
            finally
            {
                client.Dispose();
                message.Dispose();
            }
        }
    }
}
