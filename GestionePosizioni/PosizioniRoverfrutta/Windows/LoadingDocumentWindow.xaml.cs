using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

            var viewModel = new LoadingDocumentViewModel(dataStorage, WindowManager);

            DataContext = viewModel;
            
            SetDataGridBinding(viewModel);

            AddCompanyDetailsControls(dataStorage, viewModel);

            SetPropertiesBindings();
            
            SetSaveButtonBindings(viewModel);

            SetReloadButtonBinding(viewModel);

            SetConvertButtonBinding(viewModel);

            SetSendToButtonBinding(viewModel);

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
            SetButtonBinding(viewModel, SaveButton, "SaveAll", viewModel.SaveAll);

            SaveButton.SetBinding(IsEnabledProperty, new Binding
            {
                Path = new PropertyPath("EnableButtons")
            });
        }

        private void SetReloadButtonBinding(LoadingDocumentViewModel viewModel)
        {
            SetButtonBinding(viewModel, UndoButton, "Reload", viewModel.Reload);

            UndoButton.SetBinding(IsEnabledProperty, new Binding
            {
                Path = new PropertyPath("EnableButtons")
            });
        }

        private void SetPrintButtonBinding(LoadingDocumentViewModel viewModel)
        {
            SetButtonBinding(viewModel, PdfButton, "Print", viewModel.Print);
            SetButtonBinding(viewModel, ProviderPdfButton, "PrintForProvider", viewModel.PrintForProvider);
            SetButtonBinding(viewModel, CustomerPdfButton, "PrintForCustomer", viewModel.PrintForCustomer);
            PdfButton.SetBinding(IsEnabledProperty, new Binding
            {
                Path = new PropertyPath("EnableButtons")
            });
            ProviderPdfButton.SetBinding(IsEnabledProperty, new Binding
            {
                Path = new PropertyPath("EnableButtons")
            });
            CustomerPdfButton.SetBinding(IsEnabledProperty, new Binding
            {
                Path = new PropertyPath("EnableButtons")
            });
        }

        private void SetConvertButtonBinding(LoadingDocumentViewModel viewModel)
        {
            SetButtonBinding(viewModel, ConvertButton, "Convert", viewModel.Convert);

            ConvertButton.SetBinding(IsEnabledProperty, new Binding
            {
                Path = new PropertyPath("EnableButtons")
            });
        }

        private void SetSendToButtonBinding(LoadingDocumentViewModel viewModel)
        {
            SetButtonBinding(viewModel, EmailButton, "Email", viewModel.Email);
            SetButtonBinding(viewModel, ProviderEmailButton, "EmailToProvider", viewModel.EmailToProvider);
            SetButtonBinding(viewModel, CustomerEmailButton, "EmailToCustomer", viewModel.EmailToCustomer);
            EmailButton.SetBinding(IsEnabledProperty, new Binding
            {
                Path = new PropertyPath("EnableButtons")
            });
            ProviderEmailButton.SetBinding(IsEnabledProperty, new Binding
            {
                Path = new PropertyPath("EnableButtons")
            });
            CustomerEmailButton.SetBinding(IsEnabledProperty, new Binding
            {
                Path = new PropertyPath("EnableButtons")
            });
        }
    }
}
