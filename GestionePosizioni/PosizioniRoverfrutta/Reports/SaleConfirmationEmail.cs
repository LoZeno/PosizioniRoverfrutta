using Models.DocumentTypes;
using ReportManager;

namespace PosizioniRoverfrutta.Reports
{
    public class SaleConfirmationEmail : CreateEmail<SaleConfirmation>
    {
        public void AddLogo(string base64Logo)
        {
            AddToViewBag("Base64Logo", base64Logo);
        }

        public override string TemplatePath()
        {
            return @".\Content\EmailTemplates\ConfermaDatiTrasportatore.html";
        }
    }
}