using PDCoreNew.Models;
using System.Net.Mail;
using System.Threading.Tasks;

namespace PDCoreNew.Services.IServ
{
    public interface IMailServiceAsyncTask : IMailServiceAsync
    {
        Task SendEmailAsyncTask(MailMessageModel mailMessageModel, SmtpSettingsModel smtpSettingsModel = null);

        Task SendEmailAsyncTask(MailMessage message, SmtpSettingsModel smtpSettingsModel = null);

        Task SendEmailAsyncTask(MailMessage message);

        Task SendEmailAsyncTask(MailMessage message, SmtpClient client);
    }
}
