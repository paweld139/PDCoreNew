using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PDCoreNew.Helpers.Translation;

namespace PDWebCoreNewNew.Extensions
{
    public static class IdentityExtensions
    {
        public static void AddErrors(this IdentityResult result, Translator translator, ModelStateDictionary modelState)
        {
            foreach (var error in result.Errors)
            {
                modelState.AddModelError(string.Empty, translator.TranslateText(error.Description));
            }
        }
    }
}
