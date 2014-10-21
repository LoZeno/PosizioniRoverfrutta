using Models.DocumentTypes;
using ReportManager;

namespace PosizioniRoverfrutta.Reports
{
    public class PriceConfirmationReport : ReportGeneratorBase<PriceConfirmation>
    {
        public PriceConfirmationReport(PriceConfirmation model, string destinationPath)
            : base(model, destinationPath)
        {
        }

        public override string TemplatePath()
        {
            return @".\Content\ReportTemplates\ConfermaPrezziModello.html";
        }
    }
}