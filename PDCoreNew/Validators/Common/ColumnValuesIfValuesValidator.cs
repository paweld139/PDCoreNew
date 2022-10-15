using PDCoreNew.Extensions;
using PDCoreNew.Models.Sheet;
using System.Collections.Generic;

namespace PDCoreNew.Validators.Common
{
    public abstract class ColumnValuesIfValuesValidator<TSheetModel, TModel> : Validator<TSheetModel>
        where TSheetModel : ImportSheetModel<TModel>
    {
        protected abstract char CellCharIf { get; }

        protected abstract string ValueIf { get; }

        protected abstract IEnumerable<KeyValuePair<char, string>> ChellCharsValuesExpected { get; }

        protected override bool ExecuteValidate(TSheetModel input)
        {
            var (sheet, headerRowInt, _) = input;

            return sheet.ValidateColumnValuesIfValues(CellCharIf, headerRowInt, ValueIf, ChellCharsValuesExpected);
        }
    }
}
