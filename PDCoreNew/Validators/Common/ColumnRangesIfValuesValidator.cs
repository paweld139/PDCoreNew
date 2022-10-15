using PDCoreNew.Extensions;
using PDCoreNew.Models.Sheet;
using System;

namespace PDCoreNew.Validators.Common
{
    public abstract class ColumnRangesIfValuesValidator<TSheetModel, TModel> : Validator<TSheetModel>
        where TSheetModel : ImportSheetModel<TModel>
    {
        protected abstract char CellCharIf { get; }

        protected abstract string ValueIf { get; }

        protected abstract Tuple<char, char, decimal?, decimal?, bool, bool> CellCharsRangeExpected { get; }

        protected override bool ExecuteValidate(TSheetModel input)
        {
            var (sheet, headerRowInt, _) = input;

            return sheet.ValidateColumnRangesIfValues(CellCharIf, headerRowInt, ValueIf, CellCharsRangeExpected);
        }
    }
}
