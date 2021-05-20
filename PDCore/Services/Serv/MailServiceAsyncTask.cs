using PDCore.Interfaces;
using PDCore.Models;
using PDCore.Services.IServ;
using System;
using System.ComponentModel;
using System.Net.Mail;
using System.Threading.Tasks;
using Unity;

namespace PDCore.Services.Serv
{
    public class MailServiceAsyncTask : MailServiceAsync, IMailServiceAsyncTask
    {
        public MailServiceAsyncTask(SmtpSettingsModel smtpSettingsModel, ILogger logger) : base(smtpSettingsModel, logger)
        {
        }

        [InjectionConstructor]
        public MailServiceAsyncTask(ILogger logger) : base(logger)
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


                        logger.Info(string.Format(SendStatusMessageFormat, "Sending async", message.To, message.Subject));


                        await sendMailTask;


                        OnSendCompleted(sendMailTask, message);
                    }
                    catch (Exception ex)
                    {
                        logger.Fatal("Async email error", ex);
                    }
                }
            }
        }

        private void OnSendCompleted(Task sendMailTask, MailMessage mailMessage)
        {
            AsyncCompletedEventArgs args = new AsyncCompletedEventArgs(sendMailTask?.Exception, sendMailTask?.IsCanceled ?? false, mailMessage);

            SendCompletedCallback(this, args);
        }
    }
}
