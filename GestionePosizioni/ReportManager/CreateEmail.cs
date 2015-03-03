using System.IO;
using RazorEngine;
using RazorEngine.Templating;

namespace ReportManager
{
    public abstract class CreateEmail<T>
    {
        protected CreateEmail(T model, string destinationPath)
        {
            _model = model;
            _destinationPath = destinationPath;
            _viewBag = new DynamicViewBag();
        }

        protected void AddToViewBag(string property, object value)
        {
            _viewBag.AddValue(property, value);
        }

        public void GenerateEmail()
        {
            var htmlMail = Razor.Parse<T>(LoadTemplate(), _model, _viewBag, null);
            File.WriteAllBytes(_destinationPath, System.Text.Encoding.ASCII.GetBytes(htmlMail));
        }

        private string LoadTemplate()
        {
            using (var streamReader = new StreamReader(TemplatePath(), System.Text.Encoding.UTF8))
            {
                return streamReader.ReadToEnd();
            }
        }

        public abstract string TemplatePath();

        private T _model;
        private readonly string _destinationPath;
        private DynamicViewBag _viewBag;
    }
}