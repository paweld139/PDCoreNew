using Microsoft.Extensions.Localization;
using PDCore.Services.Serv;
using PDWebCoreNew.Extensions;
using System.ComponentModel.DataAnnotations;

namespace PDWebCoreNew.Extensions
{
    public static class ValidationExtensions
    {
        public static string GetLocalizedError(this ValidationContext validationContext, string errorNessage)
        {
            var stringLocalizerFactory = (IStringLocalizerFactory)validationContext.GetService(typeof(IStringLocalizerFactory));

            var localizationService = new LocalizationService(stringLocalizerFactory);

            var localizedError = localizationService.GetLocalizedString(errorNessage);

            return localizedError;
        }

        public static void PrepareValidationResult(this ValidationAttribute validationAttribute, ValidationContext validationContext, string errorKey)
        {
            var localizedError = validationContext.GetLocalizedError(errorKey);

            validationAttribute.ErrorMessage = localizedError;
        }
    }
}

namespace System.ComponentModel.DataAnnotations
{
    public class LocalizedStringLengthColorAttribute : LocalizedStringLengthEqualAttribute
    {
        public LocalizedStringLengthColorAttribute() : base(7)
        {
        }
    }

    public class LocalizedStringLengthEqualAttribute : StringLengthAttribute
    {
        private const string errorKey = "StringLength_Equal";

        public LocalizedStringLengthEqualAttribute(int length) : base(length)
        {
            MinimumLength = length;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            this.PrepareValidationResult(validationContext, errorKey);

            return base.IsValid(value, validationContext);
        }
    }

    public sealed class LocalizedStringLengthMaxAttribute : StringLengthAttribute
    {
        private const string errorKey = "StringLength_Less";

        public LocalizedStringLengthMaxAttribute(int maximumLength) : base(maximumLength)
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            this.PrepareValidationResult(validationContext, errorKey);

            return base.IsValid(value, validationContext);
        }
    }

    public sealed class LocalizedRangeIntAttribute : LocalizedRangeAttribute
    {
        public LocalizedRangeIntAttribute() : base(1, int.MaxValue)
        {
        }
    }

    public sealed class LocalizedRangeInfinity : LocalizedRangeAttribute
    {
        public LocalizedRangeInfinity() : base(1, double.PositiveInfinity)
        {
        }
    }

    public class LocalizedRangeAttribute : RangeAttribute
    {
        private const string errorKey = "Range";

        public LocalizedRangeAttribute(double minimum, double maximum) : base(minimum, maximum)
        {
        }

        public LocalizedRangeAttribute(Type type, string minimum, string maximum) : base(type, minimum, maximum)
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            this.PrepareValidationResult(validationContext, errorKey);

            return base.IsValid(value, validationContext);
        }
    }

    public sealed class LocalizedMinLengthAttribute : MinLengthAttribute
    {
        private const string errorKey = "MinLength";

        public LocalizedMinLengthAttribute(int length) : base(length)
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            this.PrepareValidationResult(validationContext, errorKey);

            return base.IsValid(value, validationContext);
        }
    }

    public sealed class LocalizedRequiredAttribute : RequiredAttribute
    {
        private const string errorKey = "RequiredError";

        public LocalizedRequiredAttribute()
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            this.PrepareValidationResult(validationContext, errorKey);

            return base.IsValid(value, validationContext);
        }
    }

    public sealed class LocalizedMaxLengthAttribute : MaxLengthAttribute
    {
        private const string errorKey = "MaxLength";

        public LocalizedMaxLengthAttribute(int length) : base(length)
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            this.PrepareValidationResult(validationContext, errorKey);

            return base.IsValid(value, validationContext);
        }
    }

    /// <summary>
    /// Validation attribute that demands that a boolean value must be true.
    /// </summary>
    public class EnforceTrueAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return false;

            if (value.GetType() != typeof(bool))
                throw new InvalidOperationException("can only be used on boolean properties.");

            return (bool)value == true;
        }
    }

    public sealed class LocalizedIsDateAfterAttribute : IsDateAfterAttribute
    {
        public LocalizedIsDateAfterAttribute(string testedPropertyName, bool allowEqualDates = false) : base(testedPropertyName, allowEqualDates)
        {
            ErrorMessage = "InvalidError";
        }
    }

    public class IsDateAfterAttribute : ValidationAttribute
    {
        private readonly string testedPropertyName;
        private readonly bool allowEqualDates;

        public IsDateAfterAttribute(string testedPropertyName, bool allowEqualDates = false)
        {
            this.testedPropertyName = testedPropertyName;
            this.allowEqualDates = allowEqualDates;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var localizedError = validationContext.GetLocalizedError(ErrorMessage);


            var propertyTestedInfo = validationContext.ObjectType.GetProperty(testedPropertyName);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", testedPropertyName));
            }

            if (value == null || !(value is DateTime))
            {
                return ValidationResult.Success;
            }

            var propertyTestedValue = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

            if (propertyTestedValue == null || !(propertyTestedValue is DateTime))
            {
                return ValidationResult.Success;
            }

            // Compare values
            if ((DateTime)value >= (DateTime)propertyTestedValue)
            {
                if (allowEqualDates && ((DateTime)value).Date == ((DateTime)propertyTestedValue).Date)
                {
                    return ValidationResult.Success;
                }
                else if ((DateTime)value > (DateTime)propertyTestedValue)
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult(string.Format(localizedError, validationContext.DisplayName));
        }

    }
}
