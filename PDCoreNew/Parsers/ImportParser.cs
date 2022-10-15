using NPOI.SS.UserModel;
using PDCoreNew.Contracts;
using PDCoreNew.Extensions;
using PDCoreNew.Models.Sheet;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDCoreNew.Parsers
{
    public abstract class ImportParser<TData, TModel, TSheetModel> : IImportParser<TData, TModel, TSheetModel>
        where TSheetModel : ImportSheetModel<TModel>
    {
        protected abstract TData Parse(TModel model, IRow row);

        private IEnumerable<TData> Parse(TSheetModel importSheetModel, IEnumerable<IRow> rows)
        {
            var model = importSheetModel.Model;

            foreach (var row in rows)
            {
                yield return Parse(model, row);
            }
        }

        public IList<TData> Parse(TSheetModel importSheetModel)
        {
            var (sheet, headerRowInt, _) = importSheetModel;

            int startRowInt = headerRowInt + 1;

            var rows = sheet.EnumerateRows(startRowInt);

            var data = Parse(importSheetModel, rows);

            return data.ToList();
        }

        public Task<IList<TData>> ParseAsyncTask(TSheetModel importSheetModel)
        {
            var result = Parse(importSheetModel);

            return Task.FromResult(result);
        }
    }
}
