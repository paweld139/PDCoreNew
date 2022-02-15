using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PDCoreNew.Utils;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IOUtils = PDWebCoreNewNew.Utils.IOUtils;

namespace PDWebCoreNewNew.Extensions
{
    public static class HttpContextExtensions
    {
        public static bool StartsWithSegment(this HttpContext httpContext, string value)
        {
            return httpContext.Request.Path.StartsWithSegments(new PathString("/" + value), StringComparison.InvariantCulture);
        }

        public static string GetString(this HttpContext httpContext, string key)
        {
            return httpContext?.Session?.GetString(key);
        }

        public static T GetObject<T>(this HttpContext httpContext, string key)
        {
            var sessionValue = httpContext.GetString(key);

            var value = JsonConvert.DeserializeObject(sessionValue);

            return value == null ? default : (T)value;
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
            using var memoryStream = new MemoryStream();

            await formFile.CopyToAsync(memoryStream);

            return memoryStream.ToArray();
        }

        public static ClaimsPrincipal GetUser(this IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor.HttpContext?.User;
        }
    }
}
