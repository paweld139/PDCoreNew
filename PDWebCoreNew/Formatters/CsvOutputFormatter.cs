using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using PDCore.Extensions;
using PDCore.Utils;
using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace PDWebCoreNew.Formatters
{
    public class CsvOutputFormatter : TextOutputFormatter
    {
        public CsvOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        protected override bool CanWriteType(Type type)
        {
            return type.ImplementsInterface<IList>(); // you could be fancy here but this gets the job done.
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var response = context.HttpContext.Response;

            var result = context.Object as IEnumerable;

            string resultString = CSVUtils.GetCSV(result, selectedEncoding, CultureInfo.InvariantCulture);

            return response.WriteAsync(resultString, selectedEncoding);
        }
    }
}
