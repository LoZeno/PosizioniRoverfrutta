using Models.DocumentTypes;
using ReportManager;

namespace PosizioniRoverfrutta.Reports
{
    public class InvoiceReport : ReportGeneratorBase<SummaryAndInvoice>
    { 
        public override string TemplatePath()
        { 
            return @".\Content\ReportTemplates\FatturaModello.html";
        }
    }
}