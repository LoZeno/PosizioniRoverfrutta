using Models.DocumentTypes;
using ReportManager;

namespace PosizioniRoverfrutta.Reports
{
    public class InvoiceReport : ReportGeneratorBase<SummaryAndInvoice>
    {
        public InvoiceReport(SummaryAndInvoice model, string destinationPath) 
            : base(model, destinationPath)
        {
        }

        public override string TemplatePath()
        {
            return @".\Content\ReportTemplates\FatturaModello.html";
        }
    }
}