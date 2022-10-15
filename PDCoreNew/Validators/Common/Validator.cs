using Microsoft.Extensions.Localization;
using PDCoreNew.Contracts.Validators;
using PDCoreNew.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace PDCoreNew.Validators.Common
{
    public abstract class Validator<T> : IValidator<T>
    {
        public abstract string GetError(IStringLocalizer stringLocalizer);

        protected string GetError(IStringLocalizer stringLocalizer, string errorTranslationKey, params string[] paramsTranslationsKeys)
        {
            string error = stringLocalizer[errorTranslationKey];

            var parameters = paramsTranslationsKeys.ToArray(p => stringLocalizer[p]);

            return string.Format(error, parameters);
        }

        public abstract int Order { get; }

        public virtual ValueTask<bool> Validate(T input)
        {
            var result = ExecuteValidate(input);

            return ValueTask.FromResult(result);
        }

        protected virtual bool ExecuteValidate(T input) => true;
    }
}
