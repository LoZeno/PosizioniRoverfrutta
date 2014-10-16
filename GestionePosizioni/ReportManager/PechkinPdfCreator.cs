using System.Drawing.Printing;
using System.IO;
using Pechkin;
using RazorEngine;

namespace ReportManager
{
    public class PechkinPdfCreator
    {
        public PechkinPdfCreator(object model, string templatePath, string destinationPath)
        {
            DestinationPath = destinationPath;
            Model = model;
            TemplatePath = templatePath;
        }

        public void CreatePdf()
        {
            string template;
            using (var streamReader = new StreamReader(TemplatePath, System.Text.Encoding.UTF8))
            {
                template = streamReader.ReadToEnd();
            }

            var htmlDocument = Razor.Parse(template, Model);

            var globalConfig = new GlobalConfig();
            globalConfig.SetPaperSize(PaperKind.A4);

            byte[] pdfBuf = new SimplePechkin(globalConfig).Convert(htmlDocument);

            File.WriteAllBytes(DestinationPath, pdfBuf);
        }

        public string DestinationPath { get; private set; }
        public object Model { get; private set; }
        public string TemplatePath { get; private set; }
    }

    //public class CodaxyPdfCreator
    //{
    //    public CodaxyPdfCreator(object model, string templatePath, string destinationPath)
    //    {
    //        DestinationPath = destinationPath;
    //        Model = model;
    //        TemplatePath = templatePath;
    //    }

    //    public void CreatePdf()
    //    {
    //        string template;
    //        using (var streamReader = new StreamReader(TemplatePath, System.Text.Encoding.UTF8))
    //        {
    //            template = streamReader.ReadToEnd();
    //        }

    //        var htmlDocument = Razor.Parse(template, Model);

    //        PdfDocument document = new PdfDocument();
            
    //    }

    //    public string DestinationPath { get; private set; }
    //    public object Model { get; private set; }
    //    public string TemplatePath { get; private set; }
    //}

    //public class WKHtmlSharpPdfCreator
    //{
    //    public WKHtmlSharpPdfCreator(object model, string templatePath, string destinationPath)
    //    {
    //        DestinationPath = destinationPath;
    //        Model = model;
    //        TemplatePath = templatePath;
    //    }

    //    public void CreatePdf()
    //    {
    //        string template;
    //        using (var streamReader = new StreamReader(TemplatePath, System.Text.Encoding.UTF8))
    //        {
    //            template = streamReader.ReadToEnd();
    //        }

    //        var htmlDocument = Razor.Parse(template, Model);

    //        WkHtmlToPdfConverter converter = new WkHtmlToPdfConverter();
    //        converter.ObjectSettings.Web.LoadImages = true;

    //        byte[] pdfBuf = converter.Convert(htmlDocument);

    //        File.WriteAllBytes(DestinationPath, pdfBuf);
    //    }

    //    public string DestinationPath { get; private set; }
    //    public object Model { get; private set; }
    //    public string TemplatePath { get; private set; }
    //}
}