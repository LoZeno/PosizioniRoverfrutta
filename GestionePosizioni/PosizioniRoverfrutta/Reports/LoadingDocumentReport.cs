using Models.DocumentTypes;
using ReportManager;

namespace PosizioniRoverfrutta.Reports
{
    public class LoadingDocumentReport : ReportGeneratorBase<LoadingDocument>
    {
        public LoadingDocumentReport(LoadingDocument model, string destinationPath, bool printForProvider, bool printForCustomer)
            : base(model, destinationPath)
        {
            AddToViewBag("PrintForProvider", printForProvider);
            AddToViewBag("PrintForCustomer", printForCustomer);
        }

        public override string TemplatePath()
        {
            return @".\Content\ReportTemplates\DocumentoTrasportoModello.html";
        }
    }
}
