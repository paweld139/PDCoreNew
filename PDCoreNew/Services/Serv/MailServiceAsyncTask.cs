using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PDCoreNew.Models;
using PDCoreNew.Services.IServ;
using System;
using System.ComponentModel;
using System.Net.Mail;
using System.Threading.Tasks;

namespace PDCoreNew.Services.Serv
{
    public class MailServiceAsyncTask : MailServiceAsync, IMailServiceAsyncTask
    {
        public MailServiceAsyncTask(IOptions<SmtpSettingsModel> smtpSettingsModel, ILogger<MailService> logger) : base(smtpSettingsModel, logger)
        {
        }

        public MailServiceAsyncTask(ILogger<MailService> logger) : base(logger)
        {
        }

        public Task SendEmailAsyncTask(MailMessageModel mailMessageModel, SmtpSettingsModel smtpSettingsModel = null)
        {
            var data = GetData(mailMessageModel, smtpSettingsModel);

            return SendEmailAsyncTask(data.Item1, data.Item2);
        }

        public Task SendEmailAsyncTask(MailMessage message, SmtpSettingsModel smtpSettingsModel = null)
        {
            var client = GetSmtpClient(smtpSettingsModel);

            return SendEmailAsyncTask(message, client);
        }

        public Task SendEmailAsyncTask(MailMessage message)
        {
            return SendEmailAsyncTask(message, new SmtpClient());
        }

        public async Task SendEmailAsyncTask(MailMessage message, SmtpClient client)
        {
            using (message)
            {
                using (client)
                {
                    try
                    {
                        Task sendMailTask = client.SendMailAsync(message);


                        logger.LogInformation(SendStatusMessageFormat, "Sending async", message.To, message.Subject);


                        await sendMailTask;


                        OnSendCompleted(sendMailTask, message);
                    }
                    catch (Exception ex)
                    {
                        logger.LogCritical(ex, "Async email error");
                    }
                }
            }
        }

        private void OnSendCompleted(Task sendMailTask, MailMessage mailMessage)
        {
            AsyncCompletedEventArgs args = new(sendMailTask?.Exception, sendMailTask?.IsCanceled ?? false, mailMessage);

            SendCompletedCallback(this, args);
        }
    }
}
