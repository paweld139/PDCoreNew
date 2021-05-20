using PDWebCore.Models;

namespace PDWebCore.Factories.IFac
{
    public interface IUserDataFactory
    {
        void Fill(UserDataModel userData, string jsonString, string usersIp);
    }
}
