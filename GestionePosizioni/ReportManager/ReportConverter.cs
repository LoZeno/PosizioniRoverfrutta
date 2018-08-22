using TuesPechkin;

namespace ReportManager
{
    internal class ReportConverter
    {
        private ReportConverter()
        {
            
        }

        public static IConverter Converter { get; } = new StandardConverter(
            new PdfToolset(
                new Win64EmbeddedDeployment(
                    new TempFolderDeployment())));
    }
}