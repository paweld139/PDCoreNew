using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using PDCoreNew.Extensions;

namespace PDWebCoreNew.Extensions
{
    public static class FormFileExtensions
    {
        public static bool IsEmpty(this IFormFile formFile) => formFile == null || formFile.Length == 0;


        public static bool ValidateFileName(this IFormFile formFile, string expectedFileName)
        {
            string fileName = formFile.FileName;

            return fileName.ValidateFileName(expectedFileName);
        }


        public static ISheet GetSheet(this IFormFile formFile)
        {
            string fileName = formFile.FileName;

            using var stream = formFile.OpenReadStream();

            return stream.GetSheet(fileName);
        }
    }
}
