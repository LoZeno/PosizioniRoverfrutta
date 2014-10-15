using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Models;
using RazorEngine;

namespace ReportManager
{
    public class PdfEngine
    {
        public PdfEngine(SaleConfirmation document, string path)
        {
            _document = document;
            _path = path;
        }

        public void ExportPdf()
        {
            
            string template;
            using (var streamReader = new StreamReader(@"..\Debug\Templates\ConfermaVendita.cshtml", System.Text.Encoding.UTF8))
            {
                template = streamReader.ReadToEnd();
            }

            string htmlDocument = Razor.Parse(template, _document);

            var newDocument = new Document(PageSize.A4, 80, 80, 50, 50);
            using (var writer = PdfWriter.GetInstance(newDocument, new FileStream(_path, FileMode.Create)))
            {
                newDocument.Open();
                var reader = new StringReader(htmlDocument);
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, newDocument, reader);
                newDocument.Close();
            }
        }

        private readonly SaleConfirmation _document;
        private readonly string _path;
    }
}