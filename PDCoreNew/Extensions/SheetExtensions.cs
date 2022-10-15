using NPOI.SS.UserModel;
using PDCoreNew.Extensions;
using PDCoreNew.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDCoreNew.Extensions
{
    public static class SheetExtensions
    {
        public static IRow GetRowByRowInt(this ISheet sheet, int rowInt)
        {
            rowInt--;

            return sheet.GetRow(rowInt);
        }


        public static string GetValue(this ISheet sheet, char cellChar, int rowInt)
        {
            var row = sheet.GetRowByRowInt(rowInt);

            return row.GetValue(cellChar);
        }


        public static T GetValue<T>(this ISheet sheet, char cellChar, int rowInt)
        {
            string value = sheet.GetValue(cellChar, rowInt);

            return value.ConvertOrCastTo<string, T>();
        }


        public static int? GetValueInt(this ISheet sheet, char cellChar, int rowInt)
        {
            string value = sheet.GetValue(cellChar, rowInt);

            return value.ParseAsNullableInteger();
        }


        public static decimal? GetValueDecimal(this ISheet sheet, char cellChar, int rowInt)
        {
            string value = sheet.GetValue(cellChar, rowInt);

            return value.ParseAsNullableDecimal();
        }


        public static bool ValidateSheetName(this ISheet sheet, string expectedSheetName) => sheet.SheetName == expectedSheetName;


        public static bool ValidateValue(this ISheet sheet, char cellChar, int headerRowInt, string expectedValue)
        {
            string value = sheet.GetValue(cellChar, headerRowInt);

            return value == expectedValue;
        }


        public static bool ValidateValueInt(this ISheet sheet, char cellChar, int headerRowInt, int expectedValue)
        {
            string expectedValueString = expectedValue.ToString();

            return sheet.ValidateValue(cellChar, headerRowInt, expectedValueString);
        }


        public static bool ValidateValueIntWithHeader(this ISheet sheet, char headerCellChar, int headerRowInt,
           string expectedHeaderName, char valueCellChar, int valueRowInt, int expectedValueInt)
        {
            bool isValid = sheet.ValidateValueInt(valueCellChar, valueRowInt, expectedValueInt);

            if (isValid)
            {
                isValid = sheet.ValidateValue(headerCellChar, headerRowInt, expectedHeaderName);
            }

            return isValid;
        }


        public static IEnumerable<IRow> EnumerateRows(this ISheet sheet, int startRowInt)
        {
            int rowInt = startRowInt;

            while (true)
            {
                var row = sheet.GetRowByRowInt(rowInt);

                if (row.IsEmpty())
                {
                    break;
                }
                else
                {
                    yield return row;
                }

                rowInt++;
            }
        }


        public static bool ValidateColumn(this ISheet sheet, int headerRowInt,
            Func<IRow, bool> isRowValidFunc)
        {
            bool isValid = true;

            int startRowInt = headerRowInt + 1;

            var rows = sheet.EnumerateRows(startRowInt);

            bool rowsExists = rows.Any();

            if (!rowsExists)
            {
                isValid = false;
            }
            else
            {
                foreach (var row in rows)
                {
                    isValid = isRowValidFunc(row);

                    if (!isValid)
                    {
                        break;
                    }
                }
            }

            return isValid;
        }


        public static async ValueTask<bool> ValidateColumnValuesDictionary(this ISheet sheet, char cellChar, int headerRowInt,
            string dictionaryName, Func<string, Task<string[]>> getAllowedValues)
        {
            bool isValid = sheet.ValidateValue(cellChar, headerRowInt, dictionaryName);

            if (isValid)
            {
                var allowedValues = await getAllowedValues(dictionaryName);

                var allowedValuesHashSet = new HashSet<string>(allowedValues);

                isValid = sheet.ValidateColumn(headerRowInt, row =>
                {
                    string value = row.GetValue(cellChar);

                    return allowedValuesHashSet.Contains(value);
                });
            }

            return isValid;
        }


        public static bool ValidateColumnValuesNotEmpty(this ISheet sheet, char cellChar, int headerRowInt,
            string expectedHeaderName)
        {
            bool isValid = sheet.ValidateValue(cellChar, headerRowInt, expectedHeaderName);

            if (isValid)
            {
                isValid = sheet.ValidateColumn(headerRowInt, row =>
                {
                    string value = row.GetValue(cellChar);

                    return !value.IsNullOrWhitespace();
                });
            }

            return isValid;
        }


        public static bool ValidateColumnValuesDecimal(this ISheet sheet, char cellChar, int headerRowInt,
            string expectedHeaderName, int maxDecimalPlacesAmount)
        {
            bool isValid = sheet.ValidateValue(cellChar, headerRowInt, expectedHeaderName);

            if (isValid)
            {
                isValid = sheet.ValidateColumn(headerRowInt, row =>
                {
                    decimal? value = row.GetValueDecimal(cellChar);

                    return value != null && value.Value.GetDecimalPlacesAmount() <= maxDecimalPlacesAmount;
                });
            }

            return isValid;
        }


        public static bool ValidateColumnValuesIfValues(this ISheet sheet, char cellCharIf, int headerRowInt, string valueIf,
            IEnumerable<KeyValuePair<char, string>> cellCharsValuesExpected)
        {
            return sheet.ValidateColumn(headerRowInt, row =>
            {
                bool isValid = true;

                string currentValueIf = row.GetValue(cellCharIf);

                if (currentValueIf == valueIf)
                {
                    foreach (var cellCharValueExpected in cellCharsValuesExpected)
                    {
                        char cellCharExpected = cellCharValueExpected.Key;

                        string valueExpected = cellCharValueExpected.Value;

                        string currentValueExpected = row.GetValue(cellCharExpected);

                        isValid = currentValueExpected == valueExpected;

                        if (!isValid)
                        {
                            break;
                        }
                    }
                }

                return isValid;
            });
        }

        public static bool ValidateColumnValuesIfValues(this ISheet sheet, char cellCharIf, int headerRowInt, string valueIf,
            char cellCharExpected, string valueExpected)
        {
            var chellCharsValuesExpected = new List<KeyValuePair<char, string>>
            {
                KeyValuePair.Create(cellCharExpected, valueExpected)
            };

            return sheet.ValidateColumnValuesIfValues(cellCharIf, headerRowInt, valueIf, chellCharsValuesExpected);
        }

        public static bool ValidateColumnRangesIfValues(this ISheet sheet, char cellCharIf, int headerRowInt, string valueIf,
            IEnumerable<Tuple<char, decimal?, decimal?, bool, bool>> cellCharsRangesExpected)
        {
            return sheet.ValidateColumn(headerRowInt, row =>
            {
                bool isValid = true;

                string currentValueIf = row.GetValue(cellCharIf);

                if (currentValueIf == valueIf)
                {
                    foreach (var cellCharRangeExpected in cellCharsRangesExpected)
                    {
                        char cellCharExpected = cellCharRangeExpected.Item1;

                        decimal? rangeFromExpected = cellCharRangeExpected.Item2;

                        decimal? rangeToExpected = cellCharRangeExpected.Item3;

                        bool inclusiveFrom = cellCharRangeExpected.Item4;

                        bool inclusiveTo = cellCharRangeExpected.Item5;

                        decimal? currentValueExpected = row.GetValueDecimal(cellCharExpected);

                        isValid = currentValueExpected != null
                            && (rangeFromExpected == null || (inclusiveFrom ? currentValueExpected >= rangeFromExpected : currentValueExpected > rangeFromExpected))
                            && (rangeToExpected == null || (inclusiveTo ? currentValueExpected <= rangeToExpected : currentValueExpected < rangeToExpected));

                        if (!isValid)
                        {
                            break;
                        }
                    }
                }

                return isValid;
            });
        }

        public static bool ValidateColumnRangesIfValues(this ISheet sheet, char cellCharIf, int headerRowInt, string valueIf,
              Tuple<char, char, decimal?, decimal?, bool, bool> cellCharsRangeExpected)
        {
            char cellCharExpectedFrom = cellCharsRangeExpected.Item1;

            char cellCharExpectedTo = cellCharsRangeExpected.Item2;

            var cellCharsExpected = ObjectUtils.Range(cellCharExpectedFrom, cellCharExpectedTo);

            var cellCharsRangesExpected = cellCharsExpected
                .Select(c => Tuple.Create(c, cellCharsRangeExpected.Item3, cellCharsRangeExpected.Item4, cellCharsRangeExpected.Item5, cellCharsRangeExpected.Item6))
                .ToList();

            return sheet.ValidateColumnRangesIfValues(cellCharIf, headerRowInt, valueIf, cellCharsRangesExpected);
        }
    }
}
