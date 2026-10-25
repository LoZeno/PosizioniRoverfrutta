using System.Threading.Tasks;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace ReportManager;

internal static class ReportConverter
{
    public static async Task ConvertHtmlToPdfAsync(string html, string destinationPath, PaperFormat paperFormat)
    {
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
        await using var page = await browser.NewPageAsync();
        await page.SetContentAsync(html);
        await page.PdfAsync(destinationPath, new PdfOptions { Format = paperFormat, PrintBackground = true });
    }
}
