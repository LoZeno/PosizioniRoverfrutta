using System.Windows.Controls;
using PosizioniRoverfrutta.ViewModels;
using QueryManager;

namespace PosizioniRoverfrutta.Controls
{
    /// <summary>
    /// Interaction logic for CompanyData.xaml
    /// </summary>
    public partial class CompanyData : UserControl
    {
        public CompanyData()
        {
            InitializeComponent();
        }

        public CompanyData(IDataStorage dataStorage)
        {
            DataContext = new CustomerControlViewModel(dataStorage);
        }
    }
}
