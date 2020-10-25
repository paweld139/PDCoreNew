using PDWebCore.Models;
using System.Data.Entity;

namespace PDCore.Web.Context.IContext
{
    public interface IHasUserDataDbSet
    {
        DbSet<UserDataModel> UserData { get; set; }
    }
}
