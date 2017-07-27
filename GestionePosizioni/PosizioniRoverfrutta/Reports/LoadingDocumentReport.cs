using Models.DocumentTypes;
using RazorEngine.Templating;
using ReportManager;

namespace PosizioniRoverfrutta.Reports
{
    public class LoadingDocumentReport : ReportGeneratorBase<LoadingDocument>
    {
        public void SetPrintDestinations(bool printForProvider, bool printForCustomer)
        {
            ViewBag = new DynamicViewBag();
            AddToViewBag("PrintForProvider", printForProvider);
            AddToViewBag("PrintForCustomer", printForCustomer);
        }

        public override string TemplatePath()
        {
            return @".\Content\ReportTemplates\DocumentoTrasportoModello.html";
        }
    }
}
