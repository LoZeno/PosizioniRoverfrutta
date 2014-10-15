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
            var engine = new PdfEngine(new SaleConfirmation {Customer = new Customer {CompanyName = "HELLO COMPANY"}},
                @"C:\Users\LucaZ\Documents\parsehtml.pdf");
            engine.ExportPdf();
        }
    }
}
