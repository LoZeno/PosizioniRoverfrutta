using Models;
using Models.DocumentTypes;
using ReportManager;

namespace PosizioniRoverfrutta.Reports
{
    public class SaleConfirmationReport : ReportGeneratorBase<SaleConfirmation>
    {
        public SaleConfirmationReport(SaleConfirmation model, string destinationPath, bool printForProvider, bool printForCustomer)
            : base(model, destinationPath)
        {
            AddToViewBag("PrintForProvider", printForProvider);
            AddToViewBag("PrintForCustomer", printForCustomer);
        }

        public override string TemplatePath()
        {
            return @".\Content\ReportTemplates\ConfermaVenditaModello.html";
        }
    }
}