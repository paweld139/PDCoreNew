using Microsoft.Extensions.Localization;
using PDCoreNew.Extensions;
using PDCoreNew.Models.Sheet;
using System;

namespace PDCoreNew.Validators.Common
{
    public abstract class ValueIntWithHeaderValidator<TSheetModel, TModel> : Validator<TSheetModel>
        where TSheetModel : ImportSheetModel<TModel>
    {
        protected abstract char HeaderCellChar { get; }

        protected abstract int HeaderRowInt { get; }

        protected abstract string ExpectedHeaderName { get; }

        protected abstract char ValueCellChar { get; }

        protected abstract int ValueRowInt { get; }

        protected abstract Func<TModel, int> ExpectedValueIntFunc { get; }

        protected abstract string ExpectedHeaderNameTranslationKey { get; }

        protected override bool ExecuteValidate(TSheetModel input)
        {
            var (model, sheet) = input;

            int expectedValueInt = ExpectedValueIntFunc(model);

            return sheet.ValidateValueIntWithHeader(HeaderCellChar, HeaderRowInt, ExpectedHeaderName, ValueCellChar, ValueRowInt, expectedValueInt);
        }

        public override string GetError(IStringLocalizer stringLocalizer)
        {
            return GetError(stringLocalizer, "InvalidValueInCell", ExpectedHeaderNameTranslationKey);
        }
    }
}
