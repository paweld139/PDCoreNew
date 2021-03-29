using PDCore.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;

namespace PDCore.Utils
{
    public static class WebUtils
    {
        public const string ResultOkIndicator = "ok";

        public readonly static Dictionary<string, object> Errors = new Dictionary<string, object>
        {
            {"RNF", "Nie odnaleziono zasobu"},
            {"AD", "Nie masz dostępu do tego zasobu"},
            {"ER", "Wystąpił błąd"},
            {"NF", "Brak danych"}
        };

        public static string MakeError(string message)
        {
            return "error_" + message;
        }

        public static string MakeError(Exception exception)
        {
            return MakeError(exception.Message);
        }

        public static string MakeConfirm(string message)
        {
            return "confirm_" + message;
        }

        public static bool WithoutErrors(string result)
        {
            return result == ResultOkIndicator;
        }

        public static string JSScript(string content)
        {
            string result = string.Format("<script>{0}</script>", content);

            return result;
        }

        public static string JSScriptAlert(string text, bool historyBack = false, bool reload = false, string href = "")
        {
            string result = "<script>(function(){ alert('" + text + "');" + (historyBack == true ? "history.back();" : "") + (reload == true ? "location.reload();" : "") + (href != "" ? string.Format("location.href = '{0}';", href) : "") + " })()</script>";

            return result;
        }

        public const string br = "<br />";

        public const string hr = "<hr>";

        public static string ItallicP(string content)
        {
            string result = string.Format("<p style=\"font-style: italic\">{0}</p>", content);

            return result;
        }

        public static string Blockquote(string content)
        {
            string result = string.Format("<blockquote>{0}</blockquote>", content);

            return result;
        }

        public static string Cite(string content)
        {
            string result = string.Format("<cite>{0}</cite>", content);

            return result;
        }

        public static string Strong(string content)
        {
            string result = string.Format("<strong>{0}</strong>", content);

            return result;
        }

        public static string Tag(string name, string content)
        {
            string result = string.Format("<{0}>{1}</{0}>", name, content);

            return result;
        }


        public static string GetHTMLA(string url, string text)
        {
            string el = string.Format("<a href='{0}'>{1}</a>", url, text);

            return el;
        }

        public static string GetHTMLImg(string source, string alternativeText, string title)
        {
            string el = string.Format("<img src='{0}' alt='{1}' title='{2}'>", source, alternativeText, title);

            return el;
        }

        public static string GetHTMLIframe(string source, string alternativeText, string title)
        {
            string el = string.Format("<iframe src='{0}' title='{2}'>{1}</iframe>", source, alternativeText, title);

            return el;
        }

        public static string GetHTMLObject(string source, string alternativeText)
        {
            string el = string.Format("<object data='{0}'>{1}</object>", source, alternativeText);

            return el;
        }

        private static Task<string> DoGetTextFromWebClient(string address, WebClient webClient, bool sync)
        {
            ObjectUtils.ThrowIfNull(address.GetTuple(nameof(address)), webClient.GetTuple(nameof(webClient)));

            if (sync)
                return Task.FromResult(webClient.DownloadString(address));

            return webClient.DownloadStringTaskAsync(address);
        }

        private static async Task<string> DoGetTextFromWebClient(string address, bool sync)
        {
            using (WebClient webClient = GetWebClient())
            {
                Task<string> task = DoGetTextFromWebClient(address, webClient, sync);

                if (sync)
                    return task.Result;

                return await task;
            }
        }

        public static string GetTextFromWebClient(string address)
        {
            return DoGetTextFromWebClient(address, true).Result;
        }

        public static Task<string> GetTextAsyncFromWebClient(string address)
        {
            return DoGetTextFromWebClient(address, false);
        }

        public static string GetFileNameAfterRead(WebClient webClient)
        {
            string fileName = null;

            string headerContentDisposition = webClient.ResponseHeaders["Content-Disposition"];

            if (!string.IsNullOrEmpty(headerContentDisposition))
                fileName = new ContentDisposition(headerContentDisposition).FileName;

            return fileName;
        }

        private static async Task<string> DoSaveFileFromWebClient(string address, WebClient webClient, bool sync)
        {
            byte[] data;

            if (sync)
                data = webClient.DownloadData(address);
            else
                data = await webClient.DownloadDataTaskAsync(address);

            string fileName = GetFileNameAfterRead(webClient);

            string saveFileLocation = SecurityUtils.GetTempFilePath(fileName);

            if (sync)
                File.WriteAllBytes(saveFileLocation, data);
            else
                await IOUtils.WriteAllBytesAsync(saveFileLocation, data);

            return saveFileLocation;
        }

        private static async Task<string> DoSaveFileFromWebClient(string address, bool sync)
        {
            using (WebClient webClient = GetWebClient())
            {
                Task<string> task = DoSaveFileFromWebClient(address, webClient, sync);

                if (sync)
                    return task.Result;

                return await task;
            }
        }

        public static string SaveFileFromWebClient(string address, WebClient webClient)
        {
            return DoSaveFileFromWebClient(address, webClient, true).Result;
        }

        public static Task<string> SaveFileAsyncFromWebClient(string address, WebClient webClient)
        {
            return DoSaveFileFromWebClient(address, webClient, false);
        }

        public static string SaveFileFromWebClient(string address)
        {
            return DoSaveFileFromWebClient(address, true).Result;
        }

        public static Task<string> SaveFileAsyncFromWebClient(string address)
        {
            return DoSaveFileFromWebClient(address, false);
        }

        public static async Task<string[]> DownloadStringsAsync(string[] urls)
        {
            var tasks = new Task<string>[urls.Length];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = DownloadStringAsync(urls[i]);
            }

            return await Task.WhenAll(tasks);
        }

        public static async Task<string> DownloadStringAsync(string url)
        {
            //validate!
            using (var client = GetWebClient())
            {
                //optionally process and return
                return await client.DownloadStringTaskAsync(url).ConfigureAwait(false);
            }
        }

        public static byte[] DownloadData(string url)
        {
            //validate!
            using (var client = GetWebClient())
            {
                //optionally process and return
                return client.DownloadData(url);
            }
        }

        public static async Task<byte[]> DownloadDataAsync(string url)
        {
            //validate!
            using (var client = GetWebClient())
            {
                //optionally process and return
                return await client.DownloadDataTaskAsync(url).ConfigureAwait(false);
            }
        }

        public static WebClient GetWebClient()
        {
            WebClient webClient = new WebClient();

            return webClient;
        }

        public static TOutput GetResultWithRetryWeb<TInput, TOutput>(Func<TInput, TOutput> input, TInput param)
        {
            return input.Partial(param).WithRetryWeb().Item1;
        }

        public static TOutput GetResultWithRetryWeb<TInput, TInput2, TOutput>(Func<TInput, TInput2, TOutput> input, TInput param, TInput2 param2)
        {
            return input.Partial(param, param2).WithRetryWeb().Item1;
        }

        public static string GetCookie(HttpResponseMessage message)
        {
            var setCookieString = GetSetCookieHeaderString(message);
            var cookieTokens = setCookieString.Split(';');
            var firstCookie = cookieTokens.FirstOrDefault();
            var keyValueTokens = firstCookie.Split('=');
            var valueString = keyValueTokens[1];
            var cookieValue = HttpUtility.UrlDecode(valueString);

            return cookieValue;
        }

        public static string GetHeaderString(string name, HttpResponseMessage message)
        {
            message.Headers.TryGetValues(name, out var setCookie);

            return setCookie?.SingleOrDefault();
        }

        public static string GetSetCookieHeaderString(HttpResponseMessage message) => GetHeaderString("Set-Cookie", message);

        public static string GetCookieHeaderString(HttpResponseMessage message) => GetHeaderString("Cookie", message);

        public static bool AddCookiesToRequest(HttpClient httpClient, HttpResponseMessage message)
        {
            string cookie = GetSetCookieHeaderString(message) ?? GetCookieHeaderString(message);

            bool result = !string.IsNullOrEmpty(cookie);

            if (result)
            {
                httpClient.DefaultRequestHeaders.Add("Cookie", cookie);
            }

            return result;
        }
    }
}
