using System.Drawing.Printing;
using System.IO;
using Pechkin;
using RazorEngine;
using RazorEngine.Templating;

namespace ReportManager
{
    public abstract class ReportGeneratorBase<T>
    {
        protected ReportGeneratorBase(T model, string destinationPath)
        {
            _model = model;
            _destinationPath = destinationPath;
            _viewBag = new DynamicViewBag();
        }

        public abstract string TemplatePath();

        protected void AddToViewBag(string property, object value)
        {
            _viewBag.AddValue(property, value);
        }

        public void CreatePdf()
        {
            var template = LoadTemplate();
            var htmlDocument = Razor.Parse<T>(template, _model, _viewBag, null);

            var globalConfig = new GlobalConfig();
            globalConfig.SetPaperSize(PaperKind);

            var pdfBuf = new SimplePechkin(globalConfig).Convert(htmlDocument);

            File.WriteAllBytes(_destinationPath, pdfBuf);
        }

        private string LoadTemplate()
        {
            using (var streamReader = new StreamReader(TemplatePath(), System.Text.Encoding.UTF8))
            {
                return streamReader.ReadToEnd();
            }
        }

        private readonly string _destinationPath;
        private readonly T _model;
        protected PaperKind PaperKind = PaperKind.A4;
        protected readonly DynamicViewBag _viewBag;
    }
}