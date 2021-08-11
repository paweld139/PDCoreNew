using PDCoreNew.Models;
using System.Net.Mail;

namespace PDCoreNew.Services.IServ
{
    public interface IMailService
    {
        void SendEmail(MailMessageModel mailMessageModel, SmtpSettingsModel smtpSettingsModel = null);

        void SendEmail(MailMessage message, SmtpSettingsModel smtpSettingsModel = null);

        void SendEmail(MailMessage message);

        void SendEmail(MailMessage message, SmtpClient client);
    }
}