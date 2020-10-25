﻿using System;
using System.Web.Mvc;

namespace PDCore.Web.Attributes.MVC
{
    public class UtcDateTimeAttribute : CustomModelBinderAttribute
    {
        public override IModelBinder GetBinder()
        {
            return new UtcDateTimeModelBinder();
        }

        public class UtcDateTimeModelBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                if (bindingContext == null)
                {
                    throw new ArgumentNullException(nameof(bindingContext));
                }

                if (bindingContext.ModelMetadata.ModelType == typeof(DateTime))
                {
                    var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                    var str = valueProviderResult.AttemptedValue;

                    return DateTime.Parse(str).ToUniversalTime();
                }

                return null;
            }
        }
    }
}
