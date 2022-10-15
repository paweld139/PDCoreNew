using Microsoft.Extensions.Localization;
using PDCoreNew.Extensions;
using PDCoreNew.Models.Sheet;
using System.Threading.Tasks;

namespace PDCoreNew.Validators.Common
{
    public abstract class ColumnValuesDictionaryValidator<TSheetModel, TModel> : HeaderValidator<TSheetModel>
        where TSheetModel : ImportSheetModel<TModel>
    {
        public override ValueTask<bool> Validate(TSheetModel input)
        {
            var (sheet, headerRowInt, getAllowedValuesFunc) = input;

            return sheet.ValidateColumnValuesDictionary(CellChar, headerRowInt, ExpectedHeaderName, getAllowedValuesFunc);
        }

        public override string GetError(IStringLocalizer stringLocalizer)
        {
            return GetError(stringLocalizer, "IncorrectDataInColumn", ExpectedHeaderNameTranslationKey);
        }
    }
}
