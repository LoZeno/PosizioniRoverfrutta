using System.Windows;
using PosizioniRoverfrutta.Windows;
using QueryManager;

namespace PosizioniRoverfrutta
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WindowManager _windowsManager;
        public MainWindow()
        {
            InitializeComponent();
            _windowsManager = new WindowManager((IDataStorage)App.Current.Properties["DataStorage"]);
        }

        private void NewDocumentButton_OnClick(object sender, RoutedEventArgs e)
        {
            _windowsManager.InstantiateWindow("1", WindowTypes.ConfermaVendita);
        }
    }
}
