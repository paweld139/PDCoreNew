using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PDCore.Utils;
using PDWebCoreNew.Utils;
using System;
using System.Globalization;
using System.Threading.Tasks;
using IOUtils = PDWebCoreNew.Utils.IOUtils;

namespace PDWebCoreNew.ModelBinders
{
    public class UtcDateTimeModelBinder : IModelBinder
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public UtcDateTimeModelBinder(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            string modelName = bindingContext.ModelName;

            var stringValue = bindingContext.ValueProvider.GetValue(modelName);

            if (stringValue == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            if (!DateTime.TryParse(stringValue.FirstValue, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out DateTime parsedDate))
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }

            if (parsedDate.Kind != DateTimeKind.Utc)
            {
                int? timezoneOffset = IOUtils.GetTimezoneOffset(httpContextAccessor);

                if (timezoneOffset != null)
                {
                    parsedDate = DateTimeUtils.DeleteOffset(parsedDate, timezoneOffset.Value);
                }
            }

            bindingContext.Result = ModelBindingResult.Success(parsedDate);

            return Task.CompletedTask;
        }
    }
}
