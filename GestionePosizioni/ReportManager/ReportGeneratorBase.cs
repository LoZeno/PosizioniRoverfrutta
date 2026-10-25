using System.Collections.Generic;
using System.Dynamic;
using System.Drawing.Printing;
using System.IO;
using PuppeteerSharp.Media;
using RazorLight;

namespace ReportManager;

public abstract class ReportGeneratorBase<T>
{
    private readonly RazorLightEngine _engine;

    protected ReportGeneratorBase()
    {
        ViewBag = new ExpandoObject();
        // RazorLight accepts ExpandoObject as view bag
        _engine = new RazorLightEngineBuilder()
            .UseMemoryCachingProvider()
            .Build();
    }

    public abstract string TemplatePath();

    protected void AddToViewBag(string property, object value)
    {
        ((IDictionary<string, object>)(ViewBag as ExpandoObject ?? new ExpandoObject()))[property] = value;
    }

    public void CreatePdf(T model, string destinationPath)
    {
        var template = LoadTemplate();
        var htmlDocument = _engine.CompileRenderStringAsync(TemplatePath(), template, model, ViewBag as ExpandoObject)
            .GetAwaiter().GetResult();

        var format = PaperKind == PaperKind.A4 ? PaperFormat.A4 : PaperFormat.Letter;
        ReportConverter.ConvertHtmlToPdfAsync(htmlDocument, destinationPath, format).GetAwaiter().GetResult();
    }

    private string LoadTemplate()
    {
        using var streamReader = new StreamReader(TemplatePath(), System.Text.Encoding.UTF8);
        return streamReader.ReadToEnd();
    }

    protected PaperKind PaperKind = PaperKind.A4;
    protected object ViewBag;
}
