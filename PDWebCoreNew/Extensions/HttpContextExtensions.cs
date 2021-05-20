using Microsoft.AspNetCore.Http;
using PDCore.Utils;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IOUtils = PDWebCoreNew.Utils.IOUtils;

namespace PDWebCoreNew.Extensions
{
    public static class HttpContextExtensions
    {
        public static bool StartsWithSegment(this HttpContext httpContext, string value)
        {
            return httpContext.Request.Path.StartsWithSegments(new PathString("/" + value), StringComparison.InvariantCulture);
        }

        public static T GetObject<T>(this HttpContext httpContext, string key)
        {
            var objectByteArray = httpContext.Session.Get(key);

            var value = objectByteArray == null ? null : ObjectUtils.ByteArrayToObject(objectByteArray);

            return value == null ? default(T) : (T)value;
        }

        public static string[] GetUserLanguages(this HttpRequest request)
        {
            return request.GetTypedHeaders()
                .AcceptLanguage?
                .OrderByDescending(x => x.Quality ?? 1)
                .Select(x => x.Value.ToString())
                .ToArray() ?? Array.Empty<string>();
        }

        public static DateTime ApplyUtcOffset(this DateTime input, IHttpContextAccessor httpContextAccessor)
        {
            int utcOffset = IOUtils.GetTimezoneOffset(httpContextAccessor).GetValueOrDefault();

            return DateTimeUtils.ApplyOffset(input, utcOffset);
        }

        public static DateTime? DeleteUtcOffset(this DateTime? input, IHttpContextAccessor httpContextAccessor)
        {
            if (input != null)
                return input.Value.DeleteUtcOffset(httpContextAccessor);

            return input;
        }

        public static DateTime DeleteUtcOffset(this DateTime input, IHttpContextAccessor httpContextAccessor)
        {
            if (input.Kind == DateTimeKind.Unspecified)
            {
                int utcOffset = IOUtils.GetTimezoneOffset(httpContextAccessor).GetValueOrDefault();

                input = DateTimeUtils.DeleteOffset(input, utcOffset);
            }

            return input;
        }

        public static async Task<byte[]> GetBytes(this IFormFile formFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);

                return memoryStream.ToArray();
            }
        }
    }
}
