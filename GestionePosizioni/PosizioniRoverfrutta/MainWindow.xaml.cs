using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using PosizioniRoverfrutta.ViewModels;
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
        private readonly IDataStorage _dataStorage;

        public MainWindow(WindowManager windowManager, IDataStorage dataStorage)
        {
            InitializeComponent();
            _windowsManager = windowManager;
            _dataStorage = dataStorage;
            var viewModel = new ListPositionsViewModel(_dataStorage);
            this.DataContext = viewModel;
            SetDataGridBinding(viewModel);
            //this.SetBinding(Window.IsActiveProperty, new Binding());
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

        private void SummaryButton_OnClick(object sender, RoutedEventArgs e)
        {
            _windowsManager.InstantiateWindow("", WindowTypes.Riepiloghi);
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

        private void SetDataGridBinding(ListPositionsViewModel viewModel)
        {
            ListPositionsGrid.SetBinding(DataGrid.ItemsSourceProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("PositionsList"),
            });

            ListPositionsGrid.SetBinding(DataGrid.SelectedItemProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("SelectedPosition"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            });
        }
    }
}
