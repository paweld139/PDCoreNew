using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;

namespace PDCoreNew.Extensions
{
    public static class StreamExtensions
    {
        public static ISheet GetSheet(this Stream stream, string fileName)
        {
            string fileExtension = Path.GetExtension(fileName);

            string fileExtensionLower = fileExtension.ToLower();

            ISheet sheet;

            if (fileExtensionLower == ".xls") //This will read the Excel 97-2000 formats    
            {
                HSSFWorkbook hssfwb = new(stream);

                sheet = hssfwb.GetSheetAt(0);
            }
            else //This will read 2007 Excel format    
            {
                XSSFWorkbook hssfwb = new(stream);

                sheet = hssfwb.GetSheetAt(0);
            }

            return sheet;
        }
    }
}
