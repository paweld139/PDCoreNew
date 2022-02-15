using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Cryptography;
using PDCoreNew.Configuration;
using PDCoreNew.Extensions;
using PDCoreNew.Models;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;

namespace PDCoreNew.Services.Serv
{
    public class EmailSender : IEmailSender
    {
        private readonly IOptions<SmtpSettingsModel> smtpOptions;
        private readonly IOptions<DkimOptions> dkimOptions;
        private readonly ILogger<EmailSender> logger;

        public EmailSender(ILogger<EmailSender> logger, IOptions<SmtpSettingsModel> smtpOptions, IOptions<DkimOptions> dkimOptions)
        {
            this.smtpOptions = smtpOptions;
            this.dkimOptions = dkimOptions;
            this.logger = logger;
        }

        private SmtpSettingsModel SmtpSettings => smtpOptions.Value;

        private DkimOptions DkimSettings => dkimOptions.Value;


        private const string EmailInfo = " to \"{Name}<{Recipient}>\" with subject \"{Subject}\"";

        public async Task SendEmailAsync(string email, string subject, string bodyHtml, string body = null, string name = null, string attachementPath = null, string attachementName = null)
        {
            // create message
            var mimeMessage = CreateMessage(email, subject, name);

            await SetBody(bodyHtml, body, mimeMessage, attachementPath, attachementName);

            string dkimKeyPath = DkimSettings.Path;

            bool sign = !dkimKeyPath.IsNullOrWhitespace();

            if (sign)
            {
                Sign(mimeMessage, dkimKeyPath);
            }

            // send email
            using var smtp = new SmtpClient();

            try
            {
                await Send(mimeMessage, smtp);

                logger.LogInformation("Email sent successfully" + EmailInfo, name, email, subject);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error sending e-mail" + EmailInfo, name, email, subject);

                throw;
            }
            finally
            {
                smtp.Disconnect(true);
            }
        }

        private Task Send(MimeMessage mimeMessage, SmtpClient smtp)
        {
            smtp.Connect(SmtpSettings.Host, SmtpSettings.Port, SecureSocketOptions.StartTls);

            smtp.Authenticate(SmtpSettings.Login, SecureStringToString(SmtpSettings.Password));

            return smtp.SendAsync(mimeMessage);
        }

        private static async Task SetBody(string bodyHtml, string body, MimeMessage mimeMessage, string attachementPath, string attachementName)
        {
            var bodyBuilder = new BodyBuilder
            {
                TextBody = body,
                HtmlBody = bodyHtml
            };

            if (!attachementPath.IsNullOrWhitespace())
            {
                using var fileStream = File.OpenRead(attachementPath);

                await bodyBuilder.Attachments.AddAsync(attachementName, fileStream);
            }

            mimeMessage.Body = bodyBuilder.ToMessageBody();

            mimeMessage.Prepare(EncodingConstraint.SevenBit);
        }

        private MimeMessage CreateMessage(string email, string subject, string name)
        {
            var mimeMessage = new MimeMessage();

            mimeMessage.From.Add(new MailboxAddress(SmtpSettings.DisplayName, SmtpSettings.Email));

            mimeMessage.To.Add(new MailboxAddress(name, email));

            mimeMessage.Subject = subject;

            return mimeMessage;
        }

        private void Sign(MimeMessage mimeMessage, string dkimKeyPath)
        {
            string domainName = DkimSettings.DomainName;

            var headers = new[] { HeaderId.From, HeaderId.Subject, HeaderId.To, HeaderId.MessageId, HeaderId.Date };

            using var fileStream = File.OpenRead(dkimKeyPath);

            var signer = new DkimSigner(
                  stream: fileStream, // path to your privatekey
                  domainName, // your domain name
                  DkimSettings.Selector) // The selector given on https://dkimcore.org/
            {
                HeaderCanonicalizationAlgorithm = DkimCanonicalizationAlgorithm.Simple,
                BodyCanonicalizationAlgorithm = DkimCanonicalizationAlgorithm.Simple,
                AgentOrUserIdentifier = "@" + domainName, // your domain name
                QueryMethod = "dns/txt",
            };

            signer.Sign(mimeMessage, headers);
        }

        private static string SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;

            try
            {
                valuePtr = Marshal.SecureStringToBSTR(value);

                return Marshal.PtrToStringBSTR(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeBSTR(valuePtr);
            }
        }
    }
}
