using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PDCoreNew.Context.IContext;
using PDCoreNew.Interfaces;
using System;

namespace PDWebCoreNewNew.Context
{
    public class MainWebDbContext<TUser> : IdentityDbContext<TUser>, IEntityFrameworkCoreDbContext where TUser : IdentityUser
    {
        protected MainWebDbContext(DbContextOptions options) : base(options)
        {
        }

        public bool IsLoggingEnabled => throw new NotImplementedException();

        public void SetLogging(bool input, ILogger logger)
        {
            throw new NotImplementedException();
        }
    }
}
