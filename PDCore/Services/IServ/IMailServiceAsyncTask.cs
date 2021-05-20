using PDCore.Models;
using System.Net.Mail;
using System.Threading.Tasks;

namespace PDCore.Services.IServ
{
    public interface IMailServiceAsyncTask : IMailServiceAsync
    {
        Task SendEmailAsyncTask(MailMessageModel mailMessageModel, SmtpSettingsModel smtpSettingsModel = null);

        Task SendEmailAsyncTask(MailMessage message, SmtpSettingsModel smtpSettingsModel = null);

        Task SendEmailAsyncTask(MailMessage message);

        Task SendEmailAsyncTask(MailMessage message, SmtpClient client);
    }
}
