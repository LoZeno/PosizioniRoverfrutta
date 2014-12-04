using Models.DocumentTypes;
using ReportManager;

namespace PosizioniRoverfrutta.Reports
{
    public class PriceConfirmationReport : ReportGeneratorBase<PriceConfirmation>
    {
        public PriceConfirmationReport(PriceConfirmation model, string destinationPath, bool printForProvider, bool printForCustomer)
            : base(model, destinationPath)
        {
            AddToViewBag("PrintForProvider", printForProvider);
            AddToViewBag("PrintForCustomer", printForCustomer);
        }

        public override string TemplatePath()
        {
            return @".\Content\ReportTemplates\ConfermaPrezziModello.html";
        }
    }
}