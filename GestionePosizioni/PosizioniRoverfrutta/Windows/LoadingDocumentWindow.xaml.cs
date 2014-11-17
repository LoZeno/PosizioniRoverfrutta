using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using PosizioniRoverfrutta.CustomControls;
using PosizioniRoverfrutta.Services;
using PosizioniRoverfrutta.ViewModels;
using QueryManager;

namespace PosizioniRoverfrutta.Windows
{
    /// <summary>
    /// Interaction logic for LoadingDocumentWindow.xaml
    /// </summary>
    public partial class LoadingDocumentWindow : BaseWindow
    {
        public LoadingDocumentWindow()
            : this(null, null)
        {
            
        }

        public LoadingDocumentWindow(IWindowManager windowManager, IDataStorage dataStorage) : base(windowManager, dataStorage)
        {
            InitializeComponent();

            var viewModel = new LoadingDocumentViewModel(dataStorage, _windowManager);
            
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

        public LoadingDocumentWindow(IWindowManager windowManager, IDataStorage dataStorage, string documentId)
            : this(windowManager, dataStorage)
        {
            try
            {
                var myId = int.Parse(documentId);
                ((LoadingDocumentViewModel) DataContext).Id = myId;
                IdBox.IsReadOnly = true;
            }
            catch (Exception)
            {
                if (!documentId.Equals("new"))
                    StatusLabel.Content = "La distinta di carico " + documentId + " non è stata trovata.";
            }
        }

        private void SetDataGridBinding(LoadingDocumentViewModel viewModel)
        {
            ProductsGrid.SetBinding(DataGrid.ItemsSourceProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("ProductDetails"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });
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

            SetBindingsForTextBox("TransportDocument", TransportDocumentCode);

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

        private void AddCompanyDetailsControls(IDataStorage dataStorage, LoadingDocumentViewModel viewModel)
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

        private void SetSaveButtonBindings(LoadingDocumentViewModel viewModel)
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

            SaveButton.SetBinding(IsEnabledProperty, new Binding
            {
                Path = new PropertyPath("EnableButtons")
            });
        }

        private void SetReloadButtonBinding(LoadingDocumentViewModel viewModel)
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

            UndoButton.SetBinding(IsEnabledProperty, new Binding
            {
                Path = new PropertyPath("EnableButtons")
            });
        }

        private void SetPrintButtonBinding(LoadingDocumentViewModel viewModel)
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

            PdfButton.SetBinding(IsEnabledProperty, new Binding
            {
                Path = new PropertyPath("EnableButtons")
            });
        }

        private void SetConvertButtonBinding(LoadingDocumentViewModel viewModel)
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

            ConvertButton.SetBinding(IsEnabledProperty, new Binding
            {
                Path = new PropertyPath("EnableButtons")
            });
        }

        private void SetSendtButtonBinding(LoadingDocumentViewModel viewModel)
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
