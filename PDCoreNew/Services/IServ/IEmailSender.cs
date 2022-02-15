using System.Threading.Tasks;

namespace PDCoreNew.Services.Serv
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string bodyHtml, string body = null, string name = null, string attachementPath = null, string attachementName = null);
    }
}
