using System;
using System.Drawing.Printing;
using System.IO;
using RazorEngine;
using RazorEngine.Templating;
using TuesPechkin;

namespace ReportManager
{
    public abstract class ReportGeneratorBase<T>
    {
        private readonly IConverter _converter = ReportConverter.Converter;

        protected ReportGeneratorBase()
        {
            ViewBag = new DynamicViewBag();
            Engine.Razor.Compile(LoadTemplate(), TemplatePath(), typeof(T));
        }

        public abstract string TemplatePath();

        protected void AddToViewBag(string property, object value)
        {
            ViewBag.AddValue(property, value);
        }

        public void CreatePdf(T model, string destinationPath)
        {
            var htmlDocument = Engine.Razor.Run(TemplatePath(), typeof(T), model, ViewBag);

            var document = new HtmlToPdfDocument
            {
                GlobalSettings = new GlobalSettings
                {
                    ProduceOutline = true,
                    PaperSize = PaperKind,
                },
                Objects =
                {
                    new ObjectSettings
                    {
                        HtmlText = htmlDocument
                    }
                }
            };
            var pdfBuf = _converter.Convert(document);
            File.WriteAllBytes(destinationPath, pdfBuf);
        }

        private string LoadTemplate()
        {
            using (var streamReader = new StreamReader(TemplatePath(), System.Text.Encoding.UTF8))
            {
                return streamReader.ReadToEnd();
            }
        }

        protected PaperKind PaperKind = PaperKind.A4;
        protected DynamicViewBag ViewBag;
    }
}