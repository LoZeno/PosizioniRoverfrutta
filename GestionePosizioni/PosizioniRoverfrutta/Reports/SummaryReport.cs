using System.Drawing.Printing;
using Models.DocumentTypes;
using ReportManager;

namespace PosizioniRoverfrutta.Reports
{
    public class SummaryReport : ReportGeneratorBase<SummaryAndInvoice>
    {
        public SummaryReport(SummaryAndInvoice model, string destinationPath) : base(model, destinationPath)
        {
            PaperKind = PaperKind.A4Rotated;
        }

        public override string TemplatePath()
        {
            return @".\Content\ReportTemplates\RiepilogoModello.html";
        }
    }
}