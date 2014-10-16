using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using RazorEngine;

namespace ReportManager
{
    public class PdfCreator
    {
        public PdfCreator(object model, string templatePath, string cssPath, string destinationPath)
            : this(model, templatePath, destinationPath)
        {
            CssPath = cssPath;
        }

        public PdfCreator(object model, string templatePath, string destinationPath)
        {
            DestinationPath = destinationPath;
            Model = model;
            TemplatePath = templatePath;
        }

        public void GeneratePdf()
        {
            string template;
            using (var streamReader = new StreamReader(TemplatePath, System.Text.Encoding.UTF8))
            {
                template = streamReader.ReadToEnd();
            }

            var htmlDocument = Razor.Parse(template, Model);

            var newDocument = new Document(PageSize.A4, 80, 80, 50, 50);
            using (var writer = PdfWriter.GetInstance(newDocument, new FileStream(DestinationPath, FileMode.Create))
                )
            {
                newDocument.Open();
                var reader = new StringReader(htmlDocument);
                if (string.IsNullOrWhiteSpace(CssPath))
                {
                    var cssStream = new StreamReader(CssPath, System.Text.Encoding.UTF8).BaseStream;
                    var templateStream = new StreamReader(TemplatePath, System.Text.Encoding.UTF8).BaseStream;
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, newDocument, templateStream, cssStream);
                }
                else
                {
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, newDocument, reader);
                }
                
                newDocument.Close();
            }
        }

        public object Model { get; private set; }

        public string DestinationPath { get; private set; }
        public string TemplatePath { get; private set; }
        public string CssPath { get; private set; }
    }
}
