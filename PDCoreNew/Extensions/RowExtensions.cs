using NPOI.SS.UserModel;
using System.Linq;

namespace PDCoreNew.Extensions
{
    public static class RowExtensions
    {
        public static ICell GetCellForChar(this IRow row, char cellChar)
        {
            ICell result = null;

            if (row != null)
            {
                int cellInt = cellChar.GetIntForLetter();

                result = row.GetCell(cellInt);
            }

            return result;
        }

        public static string GetValue(this IRow row, char cellChar)
        {
            string result = null;

            if (row != null)
            {
                var cell = row.GetCellForChar(cellChar);

                if (cell != null)
                {
                    result = cell.CellType switch
                    {
                        CellType.Formula => cell.CachedFormulaResultType switch
                        {
                            CellType.Numeric => cell.NumericCellValue.ToString(),
                            CellType.String => cell.StringCellValue,
                            CellType.Boolean => cell.BooleanCellValue.ToString(),
                            _ => null
                        },
                        _ => cell.ToString()
                    };
                }
            }

            return result;
        }


        public static T GetValue<T>(this IRow row, char cellChar)
        {
            string value = row.GetValue(cellChar);

            return value.ConvertOrCastTo<string, T>();
        }


        public static bool IsEmpty(this IRow row) => row?.Cells.All(d => d.CellType == CellType.Blank) ?? true;


        public static int? GetValueInt(this IRow row, char cellChar)
        {
            string value = row.GetValue(cellChar);

            return value.ParseAsNullableInteger();
        }


        public static decimal? GetValueDecimal(this IRow row, char cellChar)
        {
            string value = row.GetValue(cellChar);

            return value?.ParseAsNullableDecimal();
        }
    }
}
