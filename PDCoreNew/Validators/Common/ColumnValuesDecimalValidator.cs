using Microsoft.Extensions.Localization;
using PDCoreNew.Extensions;
using PDCoreNew.Models.Sheet;

namespace PDCoreNew.Validators.Common
{
    public abstract class ColumnValuesDecimalValidator<TSheetModel, TModel> : HeaderValidator<TSheetModel>
        where TSheetModel : ImportSheetModel<TModel>
    {
        protected abstract int MaxDecimalPlacesAmount { get; }

        protected override bool ExecuteValidate(TSheetModel input)
        {
            var (sheet, headerRowInt, _) = input;

            return sheet.ValidateColumnValuesDecimal(CellChar, headerRowInt, ExpectedHeaderName, MaxDecimalPlacesAmount);
        }

        public override string GetError(IStringLocalizer stringLocalizer)
        {
            return GetError(stringLocalizer, "MissingOrIncorrectDataInColumn", ExpectedHeaderNameTranslationKey);
        }
    }
}
