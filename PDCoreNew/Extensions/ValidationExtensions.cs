using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using PDCoreNew.Extensions;
using System.ComponentModel.DataAnnotations;

namespace PDCoreNew.Extensions
{
    public static class ValidationExtensions
    {
        public static LocalizedString GetLocalizedError(this ValidationContext validationContext, string errorMessage)
        {
            var localizationService = validationContext.GetService<IStringLocalizer<Resource>>();

            var localizedError = localizationService[errorMessage];

            return localizedError;
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
        public LocalizedStringLengthEqualAttribute(int length) : base(length)
        {
            ErrorMessage = "StringLength_Equal";

            MinimumLength = length;
        }
    }

    public sealed class LocalizedStringLengthMaxAttribute : StringLengthAttribute
    {
        public LocalizedStringLengthMaxAttribute(int maximumLength) : base(maximumLength)
        {
            ErrorMessage = "StringLength_Less";
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
        private void SetData() => ErrorMessage = "Range";

        public LocalizedRangeAttribute(double minimum, double maximum) : base(minimum, maximum)
        {
            SetData();
        }

        public LocalizedRangeAttribute(Type type, string minimum, string maximum) : base(type, minimum, maximum)
        {
            SetData();
        }
    }

    public sealed class LocalizedMinLengthAttribute : MinLengthAttribute
    {
        public LocalizedMinLengthAttribute(int length) : base(length)
        {
            ErrorMessage = "MinLength";
        }
    }

    public sealed class LocalizedRequiredAttribute : RequiredAttribute
    {
        public LocalizedRequiredAttribute()
        {
            ErrorMessage = "RequiredError";
        }
    }

    public sealed class LocalizedMaxLengthAttribute : MaxLengthAttribute
    {
        public LocalizedMaxLengthAttribute(int length) : base(length)
        {
            ErrorMessage = "MaxLength";
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
                return new ValidationResult(string.Format("Unknown property {0}", testedPropertyName));
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
