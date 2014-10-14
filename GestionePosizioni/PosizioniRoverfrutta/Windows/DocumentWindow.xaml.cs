using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using PosizioniRoverfrutta.CustomControls;
using PosizioniRoverfrutta.CustomControls.DataGridColumns;
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

            var viewModel = new SaleConfirmationViewModel(dataStorage);

            BuildDataGridColumns();
            
            SetDataGridBinding(viewModel);
            
            AddCompanyDetailsControls(dataStorage, viewModel);

            DataContext = viewModel;

            SetPropertiesBindings();
            
            SetSaveButtonBindings(viewModel);

            SetStatusBinding();
        }

        public DocumentWindow(IDataStorage dataStorage, string documentId) : this (dataStorage)
        {
            try
            {
                var myId = int.Parse(documentId);
                ((SaleConfirmationViewModel) DataContext).Id = myId;
                IdBox.IsReadOnly = true;
            }
            catch (Exception)
            {
                if (!documentId.Equals("new"))
                    StatusLabel.Content = "La conferma di vendita " + documentId + " non è stata trovata.";
            }
        }

        private void SetDataGridBinding(SaleConfirmationViewModel viewModel)
        {
            ProductsGrid.SetBinding(DataGrid.ItemsSourceProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("ProductDetails"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });
        }

        private void BuildDataGridColumns()
        {
            var descriptionColumn = BuildAutocompleteBoxDataGridColumn();
            ProductsGrid.Columns.Add(descriptionColumn);

            var hiddenColumn = new DataGridTextColumn
            {
                Header = "Id Prodotto",
                Visibility = Visibility.Hidden,
                Binding = new Binding("ProductId")
                {
                    Mode = BindingMode.Default,
                    UpdateSourceTrigger = UpdateSourceTrigger.Default
                }
            };
            ProductsGrid.Columns.Add(hiddenColumn);

            var palletsColumn = BuildNumericColumn("Pallets", "Pallets");
            ProductsGrid.Columns.Add(palletsColumn);

            var packagesColumn = BuildNumericColumn("Colli", "Packages");
            ProductsGrid.Columns.Add(packagesColumn);

            var grossWeightColumn = BuildDecimalColumn("Lordo KG", "GrossWeight");
            ProductsGrid.Columns.Add(grossWeightColumn);

            var netWeightColumn = BuildDecimalColumn("Netto KG", "NetWeight");
            ProductsGrid.Columns.Add(netWeightColumn);

            var parameterColumn = BuildDecimalColumn("Parametro", "PriceParameter");
            ProductsGrid.Columns.Add(parameterColumn);

            var priceColumn = BuildDecimalColumn("Prezzo", "Price");
            ProductsGrid.Columns.Add(priceColumn);

            var currencyColumn = BuildCurrenciesDataGridColumn();
            ProductsGrid.Columns.Add(currencyColumn);

            var totalPrice = BuildReadOnlyTextColumn("Importo", "TotalPrice");
            ProductsGrid.Columns.Add(totalPrice);
        }

        private DataGridTemplateColumn BuildAutocompleteBoxDataGridColumn()
        {
            // Create The Column
            var templateColumn = new DataGridTemplateColumn
            {
                Header = "Prodotto",
                Width = new DataGridLength(3, DataGridLengthUnitType.Star)
            };

            var binding = new Binding("Description")
            {
                Mode = BindingMode.OneWay
            };

            // Create the TextBlock
            var textFactory = new FrameworkElementFactory(typeof(TextBlock));
            textFactory.SetBinding(TextBlock.TextProperty, binding);
            var textTemplate = new DataTemplate();
            textTemplate.VisualTree = textFactory;

            // Create the ComboBox
            var autocompleteBinding = new Binding("Description")
            {
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.Default
            };

            var autocompleteBoxFactory = new FrameworkElementFactory(typeof(ProductDescriptionAutocompleteBoxWrapper));
            autocompleteBoxFactory.SetBinding(ProductDescriptionAutocompleteBoxWrapper.TextProperty, autocompleteBinding);

            var autocompleteTemplate = new DataTemplate();
            autocompleteTemplate.VisualTree = autocompleteBoxFactory;

            // Set the Templates to the Column
            templateColumn.CellTemplate = textTemplate;
            templateColumn.CellEditingTemplate = autocompleteTemplate;

            return templateColumn;
        }

        private DataGridTemplateColumn BuildCurrenciesDataGridColumn()
        {
            // Create The Column
            var templateColumn = new DataGridTemplateColumn
            {
                Header = "Valuta",
                Width = new DataGridLength(1.5, DataGridLengthUnitType.Star)
            };

            var binding = new Binding("Currency")
            {
                Mode = BindingMode.OneWay
            };

            // Create the TextBlock
            var textFactory = new FrameworkElementFactory(typeof(TextBlock));
            textFactory.SetBinding(TextBlock.TextProperty, binding);
            var textTemplate = new DataTemplate();
            textTemplate.VisualTree = textFactory;

            // Create the ComboBox
            var autocompleteBinding = new Binding("Currency")
            {
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.Default
            };

            var autocompleteBoxFactory = new FrameworkElementFactory(typeof(CurrenciesAutocompleteBoxWrapper));
            autocompleteBoxFactory.SetBinding(CurrenciesAutocompleteBoxWrapper.TextProperty, autocompleteBinding);

            var autocompleteTemplate = new DataTemplate();
            autocompleteTemplate.VisualTree = autocompleteBoxFactory;

            // Set the Templates to the Column
            templateColumn.CellTemplate = textTemplate;
            templateColumn.CellEditingTemplate = autocompleteTemplate;

            return templateColumn;
        }

        private static DataGridNumericColumn BuildNumericColumn(string header, string propertyName)
        {
            return new DataGridNumericColumn
            {
                Header = header,
                Binding = new Binding(propertyName)
                {
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                },
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
            };
        }

        private static DataGridDecimalColumn BuildDecimalColumn(string header, string propertyName)
        {
            return new DataGridDecimalColumn
            {
                Header = header,
                Binding = new Binding(propertyName)
                {
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    StringFormat = "F2",
                    ConverterCulture = CultureInfo.CurrentCulture
                },
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
            };
        }

        private static DataGridTextColumn BuildReadOnlyTextColumn(string header, string propertyName)
        {
            return new DataGridTextColumn
            {
                Header = header,
                IsReadOnly = true,
                Binding = new Binding(propertyName)
                {
                    Mode = BindingMode.Default,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    StringFormat = "F2",
                    ConverterCulture = CultureInfo.CurrentCulture
                },
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
            };
        }

        private void SetPropertiesBindings()
        {
            var idBinding = new Binding("Id")
            {
                UpdateSourceTrigger = UpdateSourceTrigger.Default,
                Mode = BindingMode.TwoWay
            };
            IdBox.SetBinding(TextBox.TextProperty, idBinding);

            var dateBinding = new Binding("DocumentDate")
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            DocumentDateControl.SetBinding(DatePicker.SelectedDateProperty, dateBinding);

            SetBindingsForTotals("TotalPallets", TotalPalletsText);

            SetBindingsForTotals("TotalPackages", TotalPackagesText);

            SetBindingsForTotals("TotalGross", TotalGrossText);

            SetBindingsForTotals("TotalNet", TotalNetText);

            SetBindingsForTotals("TotalAmount", TotalAmountText);
        }

        private static void SetBindingsForTotals(string propertyName, TextBlock textBlock)
        {
            var totalsBinding = new Binding(propertyName)
            {
                UpdateSourceTrigger = UpdateSourceTrigger.Default,
                Mode = BindingMode.OneWay
            };

            textBlock.SetBinding(TextBlock.TextProperty, totalsBinding);
        }

        private void SetStatusBinding()
        {
            var statusBinding = new Binding("Status")
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.OneWay
            };
            StatusLabel.SetBinding(ContentProperty, statusBinding);
        }

        public override int Index { get; set; }

        private void AddCompanyDetailsControls(IDataStorage dataStorage, SaleConfirmationViewModel viewModel)
        {
            var customerDetailsControl = new CompanyDetails(dataStorage, viewModel.CompanyControlViewModel);
            customerDetailsControl.TitleBlock.Text = "Cliente";
            AddControlToGrid(customerDetailsControl, 0);

            var providerDetailsControl = new CompanyDetails(dataStorage, viewModel.ProviderControlViewModel);
            providerDetailsControl.TitleBlock.Text = "Fornitore";
            AddControlToGrid(providerDetailsControl, 1);

            var transporterDetailsControl = new TransporterDetails(dataStorage, viewModel.TransporterControlViewModel);
            transporterDetailsControl.TitleBlock.Text = "Trasportatore";
            AddControlToGrid(transporterDetailsControl, 2);
        }

        private void AddControlToGrid(UserControl control, int column)
        {
            Grid.SetColumn(control, column);
            CompaniesGrid.Children.Add(control);
        }

        private void SetSaveButtonBindings(SaleConfirmationViewModel viewModel)
        {
            var saveBinding = new CommandBinding
            {
                Command = viewModel.SaveAll
            };
            CommandBindings.Add(saveBinding);

            SaveButton.SetBinding(ButtonBase.CommandProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("SaveAll")
            });
        }
    }
}
