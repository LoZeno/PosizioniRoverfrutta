using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using RazorLight;

namespace ReportManager;

public abstract class CreateEmail<T>
{
    private readonly RazorLightEngine _engine;
    private readonly ExpandoObject _viewBag;

    protected CreateEmail()
    {
        _viewBag = new ExpandoObject();
        _engine = new RazorLightEngineBuilder()
            .UseMemoryCachingProvider()
            .Build();
    }

    protected void AddToViewBag(string property, object value)
    {
        ((IDictionary<string, object>)_viewBag)[property] = value;
    }

    public void GenerateEmail(T model, string destinationPath)
    {
        var template = LoadTemplate();
        var htmlMail = _engine.CompileRenderStringAsync(TemplatePath(), template, model, _viewBag)
            .GetAwaiter().GetResult();
        File.WriteAllBytes(destinationPath, System.Text.Encoding.ASCII.GetBytes(htmlMail));
    }

    private string LoadTemplate()
    {
        using var streamReader = new StreamReader(TemplatePath(), System.Text.Encoding.UTF8);
        return streamReader.ReadToEnd();
    }

    public abstract string TemplatePath();
}
