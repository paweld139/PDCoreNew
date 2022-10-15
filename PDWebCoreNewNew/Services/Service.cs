using Microsoft.AspNetCore.Http;

namespace PDWebCoreNewNew.Services
{
    public abstract class Service
    {
        protected readonly IHttpContextAccessor httpContextAccessor;

        protected Service(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        protected HttpContext HttpContext => httpContextAccessor.HttpContext;

        protected string UserName => HttpContext.User.Identity.Name;
    }
}
