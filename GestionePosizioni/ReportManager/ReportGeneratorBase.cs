using System.Drawing.Printing;
using System.IO;
using Pechkin;
using RazorEngine;

namespace ReportManager
{
    public abstract class ReportGeneratorBase<T>
    {
        protected ReportGeneratorBase(T model, string destinationPath)
        {
            _model = model;
            _destinationPath = destinationPath;
        }

        public abstract string TemplatePath();

        public void CreatePdf()
        {
            var template = LoadTemplate();

            var htmlDocument = Razor.Parse<T>(template, _model);

            var globalConfig = new GlobalConfig();
            globalConfig.SetPaperSize(PaperKind.A4);

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
    }
}