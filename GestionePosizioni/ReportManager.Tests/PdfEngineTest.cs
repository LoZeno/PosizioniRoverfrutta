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
            var engine = new PdfCreator(new SaleConfirmation { DocumentDate = DateTime.Today, Customer = new Customer { CompanyName = "HELLO COMPANY" } }, @".\Templates\ConfermaVendita.cshtml", @"C:\Users\LucaZ\Documents\parsehtml.pdf");
            engine.GeneratePdf();
        }
    }
}
