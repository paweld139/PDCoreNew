using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PDCore.Extensions;
using PDCore.Utils;
using System.Linq;

namespace PDWebCoreNew.Extensions
{
    public static class ControllerExtensions
    {
        public static string GetModelStateErrors(this ControllerBase c)
        {
            string result = string.Join(WebUtils.br, c.ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage));

            return result;
        }

        public static JObject GetModelStateErrorsJObject(this ControllerBase c)
        {
            var modelState = c.ModelState;

            var errors = new JObject();

            foreach (var key in modelState.Keys)
            {
                var state = modelState[key];

                if (state.Errors.Any())
                {
                    errors[key.ToCamelCase()] = JObject.FromObject(
                        new
                        {
                            isValid = false,
                            message = state.Errors.First().ErrorMessage
                        });
                }
            }

            return JObject.FromObject(
                new
                {
                    errors
                });
        }
    }
}
