using System.Threading.Tasks;

namespace PDCoreNew.Interfaces
{
    public interface IDataLoader
    {
        string LoadString();

        byte[] LoadBytes();

        Task<string> LoadStringAsync();

        Task<byte[]> LoadBytesAsync();
    }
}
