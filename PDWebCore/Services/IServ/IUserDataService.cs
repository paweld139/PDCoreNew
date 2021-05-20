using PDWebCore.Models;
using System.Threading.Tasks;

namespace PDWebCore.Services.IServ
{
    public interface IUserDataService
    {
        Task<UserDataModel> GetAsync();

        Task SaveAsync();
    }
}
