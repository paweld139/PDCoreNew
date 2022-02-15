using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using PDCoreNew.Configuration;
using PDCoreNew.Utils;
using PuppeteerSharp;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PDCoreNew.Extensions
{
    public static class PuppeteerExtensions
    {
        public static async Task PreparePuppeteerAsync(this IApplicationBuilder applicationBuilder, IConfiguration configuration)
        {
            _ = applicationBuilder;

            string downloadPath = null;

            var puppeteerSection = configuration.GetSection(PuppeteerOptions.Puppeteer);

            if (puppeteerSection.Exists())
            {
                var puppeteerOptions = puppeteerSection.Get<PuppeteerOptions>();

                downloadPath = puppeteerOptions.Path;
            }

            string defaultDownloadPath = Path.Join(AppContext.BaseDirectory, "puppeteer");

            var browserFetcherOptions = new BrowserFetcherOptions
            {
                Path = ObjectUtils.FirstNotNullOrWhiteSpace(downloadPath, defaultDownloadPath)
            };

            using var browserFetcher = new BrowserFetcher(browserFetcherOptions);

            var revisionInfo = await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);

            ExecutablePath = revisionInfo.ExecutablePath;
        }

        public static string ExecutablePath { get; private set; }
    }
}
