using NPOI.SS.UserModel;
using System;
using System.Threading.Tasks;

namespace PDCoreNew.Models.Sheet
{
    public class ImportSheetModel<TModel>
    {
        public ImportSheetModel(TModel model, ISheet sheet, int headerRowInt,
            Func<string, Task<string[]>> getAllowedValuesFunc)
        {
            Model = model;
            Sheet = sheet;
            HeaderRowInt = headerRowInt;
            GetAllowedValuesFunc = getAllowedValuesFunc;
        }

        public TModel Model { get; private set; }

        public ISheet Sheet { get; private set; }

        public int HeaderRowInt { get; private set; }

        public Func<string, Task<string[]>> GetAllowedValuesFunc { get; private set; }

        public void Deconstruct(out TModel model, out ISheet sheet)
        {
            model = Model;
            sheet = Sheet;
        }

        public void Deconstruct(out ISheet sheet, out int headerRowInt,
            out Func<string, Task<string[]>> getAllowedValuesFunc)
        {
            sheet = Sheet;
            headerRowInt = HeaderRowInt;
            getAllowedValuesFunc = GetAllowedValuesFunc;
        }
    }
}
