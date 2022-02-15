using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Primitives;
using PDCoreNew.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PDWebCoreNewNew.Utils
{
    public static class IOUtils
    {
        public const string TimezoneOffsetHeaderName = "xTimezoneOffset";

        public static T GetHeaderValueAs<T>(IHttpContextAccessor httpContextAccessor, string headerName)
        {
            StringValues values = default;

            if (httpContextAccessor.HttpContext?.Request?.Headers?.TryGetValue(headerName, out values) ?? false)
            {
                string rawValues = values.ToString();   // writes out as Csv when there are multiple.

                if (!rawValues.IsNullOrWhitespace())
                    return (T)Convert.ChangeType(values.ToString(), typeof(T));
            }

            return default;
        }

        public static string GetRequestIP(IHttpContextAccessor httpContextAccessor, bool tryUseXForwardHeader = true)
        {
            string ip = null;

            // todo support new "Forwarded" header (2014) https://en.wikipedia.org/wiki/X-Forwarded-For

            // X-Forwarded-For (csv list):  Using the First entry in the list seems to work
            // for 99% of cases however it has been suggested that a better (although tedious)
            // approach might be to read each IP from right to left and use the first public IP.
            // http://stackoverflow.com/a/43554000/538763
            //
            if (tryUseXForwardHeader)
                ip = GetHeaderValueAs<string>(httpContextAccessor, "X-Forwarded-For").SplitCsv().FirstOrDefault();

            // RemoteIpAddress is always null in DNX RC1 Update1 (bug).
            if (ip.IsNullOrWhitespace() && httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress != null)
                ip = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

            if (ip.IsNullOrWhitespace())
                ip = GetHeaderValueAs<string>(httpContextAccessor, "REMOTE_ADDR");

            // _httpContextAccessor.HttpContext?.Request?.Host this is the local host.

            if (ip.IsNullOrWhitespace())
                throw new Exception("Unable to determine caller's IP.");

            return ip;
        }

        public static string GetHeaderValue(HttpContext httpContext, string headerName)
        {
            string result = null;

            var headers = httpContext.Request.Headers;

            if (headers.TryGetValue(headerName, out StringValues header))
            {
                result = header;
            }

            return result;
        }

        public static string GetCookieValue(HttpContext httpContext, string cookieName)
        {
            string result = null;

            var headers = httpContext.Request.Cookies;

            if (headers.TryGetValue(cookieName, out string cookieValue))
            {
                result = cookieValue;
            }

            return result;
        }

        public static string GetHeaderValue(IHttpContextAccessor httpContextAccessor, string headerName)
        {
            return GetHeaderValue(httpContextAccessor.HttpContext, headerName);
        }

        public static string GetCookieValue(IHttpContextAccessor httpContextAccessor, string cookieName)
        {
            return GetCookieValue(httpContextAccessor.HttpContext, cookieName);
        }

        public static int? GetTimezoneOffset(IHttpContextAccessor httpContextAccessor)
        {
            int? result = null;

            string timezoneValue = GetHeaderValue(httpContextAccessor, TimezoneOffsetHeaderName);

            if (timezoneValue.IsInt(out int timezoneOffsetFromHeader))
            {
                result = timezoneOffsetFromHeader;
            }
            else
            {
                timezoneValue = GetCookieValue(httpContextAccessor, TimezoneOffsetHeaderName);

                if (timezoneValue.IsInt(out int timezoneOffsetFromCookie))
                {
                    result = timezoneOffsetFromCookie;
                }
            }

            return result;
        }

        /// <summary>
        /// Generates a Random Password
        /// respecting the given strength requirements.
        /// </summary>
        /// <param name="opts">A valid PasswordOptions object
        /// containing the password strength requirements.</param>
        /// <returns>A random password</returns>
        public static string GenerateRandomPassword(PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 8,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            string[] randomChars =
            {
                "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
                "abcdefghijkmnopqrstuvwxyz",    // lowercase
                "0123456789",                   // digits
                "!@$?_-"                        // non-alphanumeric
             };

            Random rand = new(Environment.TickCount);
            List<char> chars = new();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }
    }
}
