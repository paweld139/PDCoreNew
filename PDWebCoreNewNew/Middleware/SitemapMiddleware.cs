using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PDWebCoreNewNew.Middleware
{
    public class SitemapMiddleware
    {
        private const string SitemapFileName = "sitemap.xml";

        private readonly RequestDelegate _next;

        public SitemapMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value.Equals($"/{SitemapFileName}", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 200;

                context.Response.ContentType = "application/xml";

                string sitemapContent = await File.ReadAllTextAsync(SitemapFileName, Encoding.UTF8);

                await context.Response.WriteAsync(sitemapContent);
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
}
