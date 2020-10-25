using System.Security.Principal;
using System.Web;

namespace PDCore.Web.Models
{
    public class HttpContextPrinciple : IPrincipal
    {
        public IIdentity Identity => HttpContext.Current?.User.Identity;

        public bool IsInRole(string role) => HttpContext.Current?.User.IsInRole(role) ?? false;
    }
}
