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

            SetProductPieChartDataBindings(viewModel);
            SetCathegoriesPieChartDataBindings(viewModel);
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
            var productIdColumn = BuildReadOnlyTextColumn("ID Prodotto", "ProductId", 20, 20, true);
            CathegoriesStatisticsGrid.Columns.Add(productIdColumn);
            var descriptionColumn = BuildReadOnlyTextColumn("Categoria", "Description", 60, 160);
            CathegoriesStatisticsGrid.Columns.Add(descriptionColumn);
            var netWeightColumn = BuildReadOnlyTextColumn("Peso Netto", "NetWeight", 20, 80);
            CathegoriesStatisticsGrid.Columns.Add(netWeightColumn);
            var averagePriceColumn = BuildReadOnlyTextColumn("Prezzo Medio", "AveragePrice", 20, 80);
            CathegoriesStatisticsGrid.Columns.Add(averagePriceColumn);
            var totalAmountColumn = BuildReadOnlyTextColumn("Totale Eur", "TotalAmount", 20, 80);
            CathegoriesStatisticsGrid.Columns.Add(totalAmountColumn);
            var maximumPriceColumn = BuildReadOnlyTextColumn("Prezzo Max", "MaximumPrice", 20, 80);
            CathegoriesStatisticsGrid.Columns.Add(maximumPriceColumn);
            var minimumPriceColumn = BuildReadOnlyTextColumn("Prezzo Min", "MinimumPrice", 20, 80);
            CathegoriesStatisticsGrid.Columns.Add(minimumPriceColumn);
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

        private void SetProductPieChartDataBindings(CustomerStatisticsViewModel viewModel)
        {
            ProductChartDataSeries.DisplayMember = "Description";
            ProductChartDataSeries.ValueMember = "NetWeight";
            ProductChartDataSeries.SetBinding(ItemsControl.ItemsSourceProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("ProductStatisticsRows"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
        }

        private void SetCathegoriesPieChartDataBindings(CustomerStatisticsViewModel viewModel)
        {
            CathegoriesChartDataSeries.DisplayMember = "Description";
            CathegoriesChartDataSeries.ValueMember = "NetWeight";
            CathegoriesChartDataSeries.SetBinding(ItemsControl.ItemsSourceProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("CathegoryStatisticsRows"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
        }

        private void BuildProductsDataGridColumns()
        {
            var productIdColumn = BuildReadOnlyTextColumn("ID Prodotto", "ProductId", 20, 20, true);
            ProductStatisticsGrid.Columns.Add(productIdColumn);
            var descriptionColumn = BuildReadOnlyTextColumn("Prodotto", "Description", 60, 160);
            ProductStatisticsGrid.Columns.Add(descriptionColumn);
            var netWeightColumn = BuildReadOnlyTextColumn("Peso Netto", "NetWeight", 20, 80);
            ProductStatisticsGrid.Columns.Add(netWeightColumn);
            var averagePriceColumn = BuildReadOnlyTextColumn("Prezzo Medio", "AveragePrice", 20, 80);
            ProductStatisticsGrid.Columns.Add(averagePriceColumn);
            var totalAmountColumn = BuildReadOnlyTextColumn("Totale Eur", "TotalAmount", 20, 80);
            ProductStatisticsGrid.Columns.Add(totalAmountColumn);
            var maximumPriceColumn = BuildReadOnlyTextColumn("Prezzo Max", "MaximumPrice", 20, 80);
            ProductStatisticsGrid.Columns.Add(maximumPriceColumn);
            var minimumPriceColumn = BuildReadOnlyTextColumn("Prezzo Min", "MinimumPrice", 20, 80);
            ProductStatisticsGrid.Columns.Add(minimumPriceColumn);
        }

        private static DataGridTextColumn BuildReadOnlyTextColumn(string header, string propertyName, double size, double minimumSize, bool isHidden = false)
        {
            var column = new DataGridTextColumn
            {
                Header = header,
                IsReadOnly = true,
                Binding = new Binding(propertyName)
                {
                    Mode = BindingMode.Default,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    StringFormat = "F3",
                    ConverterCulture = CultureInfo.CurrentCulture
                },
                Width = new DataGridLength(size, DataGridLengthUnitType.Star),
                MinWidth = minimumSize,
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

        private void CathegoriesStatisticsGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CathegoriesStatisticsGrid.SelectedItem != null)
                this.CathegoryNameBox.Text = ((ProductStatistics) CathegoriesStatisticsGrid.SelectedItem).Description;
        }
    }
}
