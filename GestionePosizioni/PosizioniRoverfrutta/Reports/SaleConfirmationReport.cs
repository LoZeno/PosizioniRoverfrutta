using Models;
using Models.DocumentTypes;
using ReportManager;

namespace PosizioniRoverfrutta.Reports
{
    public class SaleConfirmationReport : ReportGeneratorBase<SaleConfirmation>
    {
        public SaleConfirmationReport(SaleConfirmation model, string destinationPath)
            : base(model, destinationPath)
        {
        }

        public override string TemplatePath()
        {
            return @".\Content\ReportTemplates\ConfermaVenditaModello.html";
        }
    }

    //public class SaleConfirmationReport : PechkinPdfCreator
    //{
    //    public SaleConfirmationReport(object model, string destinationPath)
    //        : base(model, @".\Content\ReportTemplates\ConfermaVenditaModello.html", destinationPath)
    //    {
    //    }
    //}
}