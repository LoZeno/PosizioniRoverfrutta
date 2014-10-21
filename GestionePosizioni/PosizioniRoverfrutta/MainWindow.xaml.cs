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
        private readonly WindowManager _windowsManager;
        public MainWindow(WindowManager windowManager)
        {
            InitializeComponent();
            _windowsManager = windowManager;
        }

        private void NewDocumentButton_OnClick(object sender, RoutedEventArgs e)
        {
            _windowsManager.InstantiateWindow("new", WindowTypes.ConfermaVendita);
        }

        private void SaleConfirmSearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            var documentId = SearchTextBox.Text.Trim();
            _windowsManager.InstantiateWindow(documentId, WindowTypes.ConfermaVendita);
        }

        private void ListAllDocumentsButton_OnClick(object sender, RoutedEventArgs e)
        {
            _windowsManager.InstantiateWindow("", WindowTypes.ElencoPosizioni);
        }

        private void LoadingDocumentSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var documentId = SearchTextBox.Text.Trim();
            _windowsManager.InstantiateWindow(documentId, WindowTypes.DistintaCarico);
        }

        private void PriceConfirmationSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var documentId = SearchTextBox.Text.Trim();
            _windowsManager.InstantiateWindow(documentId, WindowTypes.ConfermaPrezzi);
        }
    }
}
