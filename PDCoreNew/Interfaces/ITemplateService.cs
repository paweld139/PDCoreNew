using System.Threading.Tasks;

namespace PDCoreNew.Interfaces
{
    public interface ITemplateService
    {
        Task<string> RenderAsync<TViewModel>(string templateFileName, TViewModel viewModel);
    }
}
