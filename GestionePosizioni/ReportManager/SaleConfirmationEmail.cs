using Models.DocumentTypes;

namespace ReportManager
{
    public class SaleConfirmationEmail : CreateEmail<SaleConfirmation>
    {
        public SaleConfirmationEmail(SaleConfirmation model, string path) : base(model, path)
        {
        }

        public override string TemplatePath()
        {
            return @".\Content\EmailTemplates\ConfermaDatiTrasportatore.html";
        }
    }
}