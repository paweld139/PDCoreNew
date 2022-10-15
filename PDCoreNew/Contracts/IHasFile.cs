using Microsoft.AspNetCore.Http;

namespace PDCoreNew.Contracts
{
    public interface IHasFile
    {
        public IFormFile File { get; set; }
    }
}
