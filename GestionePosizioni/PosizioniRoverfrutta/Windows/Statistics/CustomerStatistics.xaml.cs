using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using PosizioniRoverfrutta.ViewModels.Statistics;
using QueryManager;
using System.Windows.Data;
using Models.Entities;

namespace PosizioniRoverfrutta.Windows.Statistics
{
    /// <summary>
    /// Logica di interazione per CustomerStatistics.xaml
    /// </summary>
    public partial class CustomerStatistics : BaseWindow
    {
        public CustomerStatistics()
            : this(null, null, null)
        {
            
        }

        public CustomerStatistics(IWindowManager windowManager, IDataStorage dataStorage, string customerId)
            : base(windowManager, dataStorage)
        {
            InitializeComponent();
            var viewModel = new CustomerStatisticsViewModel(dataStorage, customerId);
            this.DataContext = viewModel;

            SetBindingsForTextBlock("CustomerName", CompanyNameLabel);
            SetBindingsForDatePickers("FromDate", FromDatePicker);
            SetBindingsForDatePickers("ToDate", ToDatePicker);

            CathegoryNameBox.DataProvider = viewModel.CathegoryNamesProvider;
            var cathegoryNameBinding = new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("Cathegory"),
                UpdateSourceTrigger = UpdateSourceTrigger.Default,
                Mode = BindingMode.OneWayToSource
            };
            CathegoryNameBox.SetBinding(AutoCompleteBox.TextProperty, cathegoryNameBinding);

            SetAddToCathegoryButton(viewModel);
            SetRemoveCathegoryButton(viewModel);

            BuildProductsDataGridColumns();
            SetProductDataGridBinding(viewModel);

            BuildCathegoriesDataGridColumns();
            SetCathegoriesDataGridBinding(viewModel);
        }

        private void SetCathegoriesDataGridBinding(CustomerStatisticsViewModel viewModel)
        {
            CathegoriesStatisticsGrid.SetBinding(ItemsControl.ItemsSourceProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("CathegoryStatisticsRows"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });
        }

        private void BuildCathegoriesDataGridColumns()
        {
            var productIdColumn = BuildReadOnlyTextColumn("ID Prodotto", "ProductId", 20, true);
            CathegoriesStatisticsGrid.Columns.Add(productIdColumn);
            var descriptionColumn = BuildReadOnlyTextColumn("Categoria", "Description", 40);
            CathegoriesStatisticsGrid.Columns.Add(descriptionColumn);
            var netWeightColumn = BuildReadOnlyTextColumn("Peso Netto", "NetWeight", 20);
            CathegoriesStatisticsGrid.Columns.Add(netWeightColumn);
            var averagePriceColumn = BuildReadOnlyTextColumn("Prezzo Medio", "AveragePrice", 20);
            CathegoriesStatisticsGrid.Columns.Add(averagePriceColumn);
            var totalAmountColumn = BuildReadOnlyTextColumn("Totale Eur", "TotalAmount", 20);
            CathegoriesStatisticsGrid.Columns.Add(totalAmountColumn);
        }

        private void SetProductDataGridBinding(CustomerStatisticsViewModel viewModel)
        {
            ProductStatisticsGrid.SetBinding(ItemsControl.ItemsSourceProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("ProductStatisticsRows"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });
        }

        private void BuildProductsDataGridColumns()
        {
            var productIdColumn = BuildReadOnlyTextColumn("ID Prodotto", "ProductId", 20, true);
            ProductStatisticsGrid.Columns.Add(productIdColumn);
            var descriptionColumn = BuildReadOnlyTextColumn("Prodotto", "Description", 40);
            ProductStatisticsGrid.Columns.Add(descriptionColumn);
            var netWeightColumn = BuildReadOnlyTextColumn("Peso Netto", "NetWeight", 20);
            ProductStatisticsGrid.Columns.Add(netWeightColumn);
            var averagePriceColumn = BuildReadOnlyTextColumn("Prezzo Medio", "AveragePrice", 20);
            ProductStatisticsGrid.Columns.Add(averagePriceColumn);
            var totalAmountColumn = BuildReadOnlyTextColumn("Totale Eur", "TotalAmount", 20);
            ProductStatisticsGrid.Columns.Add(totalAmountColumn);
        }

        private static DataGridTextColumn BuildReadOnlyTextColumn(string header, string propertyName, double size, bool isHidden = false)
        {
            var column = new DataGridTextColumn
            {
                Header = header,
                IsReadOnly = true,
                Binding = new Binding(propertyName)
                {
                    Mode = BindingMode.Default,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    ConverterCulture = CultureInfo.CurrentCulture
                },
                Width = new DataGridLength(size, DataGridLengthUnitType.Star),
            };
            if (isHidden)
            {
                column.Visibility = Visibility.Collapsed;
            }
            return column;
        }

        private void SetRemoveCathegoryButton(CustomerStatisticsViewModel viewModel)
        {
            SetButtonBinding(viewModel, DeleteCathegoryButton, "RemoveCathegory", viewModel.RemoveCathegory);
        }

        private void SetAddToCathegoryButton(CustomerStatisticsViewModel viewModel)
        {
            SetButtonBinding(viewModel, AddCathegoryButton, "AddToCathegory", viewModel.AddToCathegory);
        }

        private void CustomerStats_OnChecked(object sender, RoutedEventArgs e)
        {
            if(DataContext != null)
                ((CustomerStatisticsViewModel)DataContext).CustomerOrProvider = StatisticsMode.Customer;
        }

        private void ProviderStats_OnChecked(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
                ((CustomerStatisticsViewModel) DataContext).CustomerOrProvider = StatisticsMode.Provider;
        }

        private void ProductStatisticsGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext != null)
                ((CustomerStatisticsViewModel) DataContext).SelectedProductRows = ProductStatisticsGrid.SelectedItems.OfType<ProductStatistics>().ToList();
        }
    }
}
