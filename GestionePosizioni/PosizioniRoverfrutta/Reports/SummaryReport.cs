using System.Drawing.Printing;
using Models.DocumentTypes;
using ReportManager;

namespace PosizioniRoverfrutta.Reports
{
    public class SummaryReport : ReportGeneratorBase<SummaryAndInvoice>
    {
        public SummaryReport() : base()
        {
            PaperKind = PaperKind.A4Rotated;
        }

        public override string TemplatePath()
        {
            return @".\Content\ReportTemplates\RiepilogoModello.html";
        }
    }
}