using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace PDCoreNew.Contracts.Validators
{
    public interface IValidator<T>
    {
        int Order { get; }

        ValueTask<bool> Validate(T input);

        string GetError(IStringLocalizer stringLocalizer);
    }
}
