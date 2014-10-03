using PosizioniRoverfrutta.CustomControls;
using PosizioniRoverfrutta.ViewModels;
using QueryManager;

namespace PosizioniRoverfrutta.Windows
{
    /// <summary>
    /// Interaction logic for DocumentWindow.xaml
    /// </summary>
    public partial class DocumentWindow
    {
        public DocumentWindow(IDataStorage dataStorage) : base(dataStorage)
        {
            InitializeComponent();
            MyPanel.Children.Add(new CompanyDetails(dataStorage, new CustomerControlViewModel()));
        }

        public override int Index { get; set; }
    }
}
