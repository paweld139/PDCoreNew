using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using PDCore.Extensions;
using PDCore.Utils;

namespace PDCore.Helpers.Seo
{
    public class SitemapGenerator
    {
        public const string SitemapFileName = "sitemap.xml";

        private static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        private readonly string baseUrl;
        private readonly string[] urls;

        public SitemapGenerator(string baseUrl, string[] urls)
        {
            this.baseUrl = baseUrl;
            this.urls = urls;
        }

        public async Task Generate()
        {
            XNamespace blank = "http://www.sitemaps.org/schemas/sitemap/0.9";

            var document = new XDocument(
                 new XDeclaration("1.0", "utf-8", null),
                 new XElement(blank + "urlset",
                    urls?.Select(s =>
                       new XElement(blank + "url",
                           new XElement(blank + "loc", baseUrl + s)
                       )
                    )
                 )
             );

            await semaphoreSlim.WaitAsync();

            try
            {
                string documentContent = document.ToXml();

                await IOUtils.WriteAllTextAsync(SitemapFileName, documentContent);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}
