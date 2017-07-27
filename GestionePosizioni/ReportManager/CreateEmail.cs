using System.IO;
using RazorEngine;
using RazorEngine.Templating;

namespace ReportManager
{
    public abstract class CreateEmail<T>
    {

        protected CreateEmail()
        {
            _viewBag = new DynamicViewBag();
            Engine.Razor.Compile(LoadTemplate(), TemplatePath(), typeof(T));
        }

        protected void AddToViewBag(string property, object value)
        {
            _viewBag.AddValue(property, value);
        }

        public void GenerateEmail(T model, string destinationPath)
        {
            var htmlMail = Engine.Razor.Run(TemplatePath(), typeof(T), model, _viewBag);
            File.WriteAllBytes(destinationPath, System.Text.Encoding.ASCII.GetBytes(htmlMail));
        }

        private string LoadTemplate()
        {
            using (var streamReader = new StreamReader(TemplatePath(), System.Text.Encoding.UTF8))
            {
                return streamReader.ReadToEnd();
            }
        }

        public abstract string TemplatePath();

        private readonly DynamicViewBag _viewBag;
    }
}