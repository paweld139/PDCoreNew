using Microsoft.Extensions.Localization;
using PDCoreNew.Extensions;
using PDCoreNew.Models.Sheet;

namespace PDCoreNew.Validators.Common
{
    public abstract class ColumnValuesNotEmptyValidator<TSheetModel, TModel> : HeaderValidator<TSheetModel>
        where TSheetModel : ImportSheetModel<TModel>
    {
        protected override bool ExecuteValidate(TSheetModel input)
        {
            var (sheet, headerRowInt, _) = input;

            return sheet.ValidateColumnValuesNotEmpty(CellChar, headerRowInt, ExpectedHeaderName);
        }

        public override string GetError(IStringLocalizer stringLocalizer)
        {
            return GetError(stringLocalizer, "ThereIsNoDataInColumn", ExpectedHeaderNameTranslationKey);
        }
    }
}
