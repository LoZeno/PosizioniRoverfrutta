using Models.DocumentTypes;
using ReportManager;

namespace PosizioniRoverfrutta.Reports
{
    public class LoadingDocumentReport : ReportGeneratorBase<LoadingDocument>
    {
        public LoadingDocumentReport(LoadingDocument model, string destinationPath)
            : base(model, destinationPath)
        {
        }

        public override string TemplatePath()
        {
            return @".\Content\ReportTemplates\DocumentoTrasportoModello.html";
        }
    }
}
