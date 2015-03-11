using Models.DocumentTypes;
using ReportManager;

namespace PosizioniRoverfrutta.Reports
{
    public class SaleConfirmationEmail : CreateEmail<SaleConfirmation>
    {
        public SaleConfirmationEmail(SaleConfirmation model, string path, string base64Logo) : base(model, path)
        {
            AddToViewBag("Base64Logo", base64Logo);
        }

        public override string TemplatePath()
        {
            return @".\Content\EmailTemplates\ConfermaDatiTrasportatore.html";
        }
    }
}