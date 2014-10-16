using System;
using Models;
using NUnit.Framework;

namespace ReportManager.Tests
{
    [TestFixture]
    public class PdfEngineTest
    {
        [Test]
        public void FirstTest()
        {
            var engine = new PdfCreator(new SaleConfirmation { DocumentDate = DateTime.Today, Customer = new Customer { CompanyName = "HELLO COMPANY", Address = "Via Qualcosa", VatCode = "123456789456123"} }, @".\ReportTemplates\ConfermaVenditaModello.html", @".\ReportTemplates\Style.css", @"C:\Users\LucaZ\Documents\parsehtml.pdf");
            engine.GeneratePdf();
        }

        [Test]
        public void pechkinTest()
        {
            var engine = new PechkinPdfCreator(new SaleConfirmation { DocumentDate = DateTime.Today, Customer = new Customer { CompanyName = "HELLO COMPANY", Address = "Via Qualcosa", VatCode = "123456789456123" } }, @".\ReportTemplates\ConfermaVenditaModello.html", @"C:\Users\LucaZ\Documents\parsehtml.pdf");
            engine.CreatePdf();
        }


        //[Test]
        //public void SharpTest()
        //{
        //    var engine = new WKHtmlSharpPdfCreator(new SaleConfirmation { DocumentDate = DateTime.Today, Customer = new Customer { CompanyName = "HELLO COMPANY", Address = "Via Qualcosa", VatCode = "123456789456123" } }, @".\ReportTemplates\ConfermaVenditaModello.html", @"C:\Users\LucaZ\Documents\parsehtml.pdf");
        //    engine.CreatePdf();
        //}
    }
}
