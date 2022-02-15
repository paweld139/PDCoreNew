using PDCoreNew.Extensions;
using PDCoreNew.Interfaces;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.Threading.Tasks;

namespace PDCoreNew.Services.Generator
{
    public class HtmlToPdfGenerator
    {
        private readonly ITemplateService _templateService;

        public HtmlToPdfGenerator(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        public async Task<byte[]> GetData<TViewModel>(string templateFileName, TViewModel viewModel, PdfOptions pdfOptions = null)
        {
            pdfOptions ??= new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
                MarginOptions = new MarginOptions
                {
                    Top = "2cm",
                    Left = "2cm",
                    Bottom = "2cm",
                    Right = "2cm"
                },
                DisplayHeaderFooter = true,
                HeaderTemplate = "<div></div>",
                FooterTemplate = "<div style=\"text-align: right; font-size: 8pt;width: 100%\"><span style=\"margin-right: 1.5cm; font-family:Calibri, sans-serif;\" class=\"pageNumber\"></span></div>"
            };

            var html = await _templateService.RenderAsync(templateFileName, viewModel);

            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = new string[] { "--no-sandbox", "--font-render-hinting=none" },
                ExecutablePath = PuppeteerExtensions.ExecutablePath
            });

            await using var page = await browser.NewPageAsync();

            await page.EmulateMediaTypeAsync(MediaType.Screen);

            await page.SetContentAsync(html);

            return await page.PdfDataAsync(pdfOptions);
        }
    }
}
