using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using PosizioniRoverfrutta.ViewModels;
using QueryManager;

namespace PosizioniRoverfrutta.Windows
{
    /// <summary>
    /// Interaction logic for ListPositions.xaml
    /// </summary>
    public partial class ListPositions : Window
    {
        private readonly IDataStorage _dataStorage;

        public ListPositions()
        {
            InitializeComponent();
        }

        public ListPositions(IDataStorage dataStorage, string documentId)
        {
            _dataStorage = dataStorage;
            InitializeComponent();

            var viewModel = new ListPositionsViewModel(_dataStorage);
            DataContext = viewModel;
            var newBinding = new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("Positions"),
                UpdateSourceTrigger = UpdateSourceTrigger.Default,
                Mode = BindingMode.OneTime
            };
            PositionsListBox.SetBinding(ListBox.ItemsSourceProperty, newBinding);
        }
    }
}
