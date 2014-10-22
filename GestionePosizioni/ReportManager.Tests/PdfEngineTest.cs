using System;
using Models.Companies;
using Models.DocumentTypes;
using NUnit.Framework;

namespace ReportManager.Tests
{
    [TestFixture]
    public class PdfEngineTest
    {

        [Test]
        public void pechkinTest()
        {
            var engine = new PechkinPdfCreator(new SaleConfirmation { ShippingDate = DateTime.Today, Customer = new Customer { CompanyName = "HELLO COMPANY", Address = "Via Qualcosa", VatCode = "123456789456123" } }, @".\ReportTemplates\ConfermaVenditaModello.html", @"D:\LoZeno\Documenti\parsehtml.pdf");
            engine.CreatePdf();
        }

    }
}
