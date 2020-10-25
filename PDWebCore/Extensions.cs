using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace PDWebCore
{
    public static class Extensions
    {
        public static string UrlEncodeUppercaseUTF8(this string content)
        {
            string lower = HttpUtility.UrlEncode(content, Encoding.UTF8);

            Regex reg = new Regex(@"%[a-f0-9]{2}");

            string upper = reg.Replace(lower, m => m.Value.ToUpperInvariant());

            return upper;
        }

        private const string LogId = "LOG_ID";

        public static void SetLogId(this HttpRequestMessage request, Guid id)
        {
            request.Properties[LogId] = id;
        }

        public static Guid GetLogId(this HttpRequestMessage request)
        {
            if (request.Properties.TryGetValue(LogId, out object value))
            {
                return (Guid)value;
            }

            return Guid.Empty;
        }
    }
}
