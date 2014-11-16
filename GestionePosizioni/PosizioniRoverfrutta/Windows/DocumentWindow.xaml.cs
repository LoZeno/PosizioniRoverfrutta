using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using PosizioniRoverfrutta.CustomControls;
using PosizioniRoverfrutta.CustomControls.DataGridColumns;
using PosizioniRoverfrutta.Services;
using PosizioniRoverfrutta.ViewModels;
using QueryManager;

namespace PosizioniRoverfrutta.Windows
{
    /// <summary>
    /// Interaction logic for DocumentWindow.xaml
    /// </summary>
    public partial class DocumentWindow
    {
        public DocumentWindow()
            : this(null, null)
        {
            
        }

        public DocumentWindow(IWindowManager windowManager, IDataStorage dataStorage) : base(windowManager, dataStorage)
        {
            InitializeComponent();

            var viewModel = new SaleConfirmationViewModel(dataStorage, _windowManager);

            BuildDataGridColumns();
            
            SetDataGridBinding(viewModel);
            
            AddCompanyDetailsControls(dataStorage, viewModel);

            DataContext = viewModel;

            SetPropertiesBindings();
            
            SetSaveButtonBindings(viewModel);

            SetReloadButtonBinding(viewModel);

            SetConvertButtonBinding(viewModel);

            SetSendtButtonBinding(viewModel);

            SetStatusBinding();

            SetPrintButtonBinding(viewModel);
        }

        public DocumentWindow(IWindowManager windowManager, IDataStorage dataStorage, string documentId)
            : this(windowManager, dataStorage)
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

            var binding = new Binding("ProductId")
            {
                Mode = BindingMode.Default,
                UpdateSourceTrigger = UpdateSourceTrigger.Default,
            };
            binding.ValidationRules.Add(new ExceptionValidationRule());
            var hiddenColumn = new DataGridTextColumn
            {
                Header = "Id Prodotto",
                Visibility = Visibility.Hidden,
                Binding = binding
            };
            ProductsGrid.Columns.Add(hiddenColumn);

            var palletsColumn = BuildDecimalColumn("Pallets", "Pallets");
            ProductsGrid.Columns.Add(palletsColumn);

            var packagesColumn = BuildNumericColumn("Colli", "Packages");
            ProductsGrid.Columns.Add(packagesColumn);

            var grossWeightColumn = BuildDecimalColumn("Lordo KG", "GrossWeight");
            ProductsGrid.Columns.Add(grossWeightColumn);

            var netWeightColumn = BuildDecimalColumn("Netto KG", "NetWeight");
            ProductsGrid.Columns.Add(netWeightColumn);

            var parameterColumn = BuildDecimalColumn("Parametro", "PriceParameter");
            ProductsGrid.Columns.Add(parameterColumn);

            var priceColumn = BuildPriceColumn("Prezzo", "Price");
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

        private static DataGridColumn BuildNumericColumn(string header, string propertyName)
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

        private static DataGridColumn BuildDecimalColumn(string header, string propertyName)
        {
            var binding = new Binding(propertyName)
            {
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus,
                StringFormat = "{0:0.##}",
                ConverterCulture = CultureInfo.CurrentCulture
            };
            binding.ValidationRules.Add(new ExceptionValidationRule());
            return new DataGridTextColumn
            {
                Header = header,
                Binding = binding,
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
            };
        }

        private static DataGridColumn BuildPriceColumn(string header, string propertyName)
        {
            var binding = new Binding(propertyName)
            {
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus,
                StringFormat = "F",
                ConverterCulture = CultureInfo.CurrentCulture
            };
            binding.ValidationRules.Add(new ExceptionValidationRule());
            return new DataGridTextColumn
            {
                Header = header,
                Binding = binding,
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
                    StringFormat = "F",
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

            SetBindingsForDatePickers("DocumentDate", DocumentDatePicker);

            SetBindingsForDatePickers("ShippingDate", ShippingDatePicker);
            
            SetBindingsForDatePickers("DeliveryDate", DeliveryDatePicker);

            SetBindingsForTextBox("TruckLicensePlate", LicensePlate);

            SetBindingsForNumericTextBox("Rental", Rental);

            SetBindingsForTextBox("DeliveryEx", DeliveryEx);

            SetBindingForTermsOfPaymentAutocomplete();

            SetBindingsForNumericTextBox("InvoiceDiscount", Discount);

            SetBindingsForNumericTextBox("CustomerCommission", CustomerCommission);

            SetBindingsForNumericTextBox("ProviderCommission", ProviderCommission);

            SetBindingsForDecimalTotals("TotalPallets", TotalPalletsText);

            SetBindingsForTotals("TotalPackages", TotalPackagesText);

            SetBindingsForDecimalTotals("TotalGross", TotalGrossText);

            SetBindingsForDecimalTotals("TotalNet", TotalNetText);

            SetBindingsForPriceTotals("TotalAmount", TotalAmountText);

            SetBindingsForTextBox("Notes", Notes);

            SetBindingsForTextBox("Lot", Lot);

            SetBindingsForTextBox("OrderCode", OrderCode);
        }

        private void SetBindingForTermsOfPaymentAutocomplete()
        {
            var provider = new TermsOfPaymentAutoCompleteBoxProvider(DataStorage);
            TermsOfPayment.AutoCompleteManager.DataProvider = provider;
            TermsOfPayment.AutoCompleteManager.Asynchronous = true;
            TermsOfPayment.AutoCompleteManager.AutoAppend = true;

            var binding = new Binding("TermsOfPayment")
            {
                UpdateSourceTrigger = UpdateSourceTrigger.Default,
                Mode = BindingMode.TwoWay
            };
            TermsOfPayment.SetBinding(ComboBox.TextProperty, binding);
        }

        private static void SetBindingsForTextBox(string property, TextBox control)
        {
            var binding = new Binding(property)
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            control.SetBinding(TextBox.TextProperty, binding);
        }
        
        private static void SetBindingsForNumericTextBox(string property, TextBox control)
        {
            var binding = new Binding(property)
            {
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus,
                Mode = BindingMode.TwoWay,
                ConverterCulture = CultureInfo.CurrentCulture
            };
            binding.ValidationRules.Add(new ExceptionValidationRule());
            control.SetBinding(TextBox.TextProperty, binding);
        }

        private static void SetBindingsForDatePickers(string property, DatePicker datePicker)
        {
            var dateBinding = new Binding(property)
            {
                UpdateSourceTrigger = UpdateSourceTrigger.Default,
                Mode = BindingMode.TwoWay
            };
            datePicker.SetBinding(DatePicker.SelectedDateProperty, dateBinding);
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

        private static void SetBindingsForDecimalTotals(string propertyName, TextBlock textBlock)
        {
            var totalsBinding = new Binding(propertyName)
            {
                UpdateSourceTrigger = UpdateSourceTrigger.Default,
                Mode = BindingMode.OneWay,
                StringFormat = "{0:0.##}",
                ConverterCulture = CultureInfo.CurrentCulture
            };

            textBlock.SetBinding(TextBlock.TextProperty, totalsBinding);
        }
        private static void SetBindingsForPriceTotals(string propertyName, TextBlock textBlock)
        {
            var totalsBinding = new Binding(propertyName)
            {
                UpdateSourceTrigger = UpdateSourceTrigger.Default,
                Mode = BindingMode.OneWay,
                StringFormat = "F2",
                ConverterCulture = CultureInfo.CurrentCulture
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

        private void SetReloadButtonBinding(SaleConfirmationViewModel viewModel)
        {
            var reloadBinding = new CommandBinding
            {
                Command = viewModel.Reload
            };
            CommandBindings.Add(reloadBinding);

            UndoButton.SetBinding(ButtonBase.CommandProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("Reload")
            });
        }

        private void SetPrintButtonBinding(SaleConfirmationViewModel viewModel)
        {
            var printBinding = new CommandBinding
            {
                Command = viewModel.Print
            };
            CommandBindings.Add(printBinding);

            PdfButton.SetBinding(ButtonBase.CommandProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("Print")
            });
        }

        private void SetConvertButtonBinding(SaleConfirmationViewModel viewModel)
        {
            var convertBinding = new CommandBinding
            {
                Command = viewModel.Convert
            };
            CommandBindings.Add(convertBinding);

            ConvertButton.SetBinding(ButtonBase.CommandProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("Convert")
            });
        }

        private void SetSendtButtonBinding(SaleConfirmationViewModel viewModel)
        {
            var sendBinding = new CommandBinding
            {
                Command = viewModel.Email
            };
            CommandBindings.Add(sendBinding);

            EmailButton.SetBinding(ButtonBase.CommandProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("Email")
            });
        }
    }
}
