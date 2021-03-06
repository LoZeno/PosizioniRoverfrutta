﻿using System;
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

            var viewModel = new SaleConfirmationViewModel(dataStorage, WindowManager);
            
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

            SetCloneButtonBinding(viewModel);

            SetAttachmentButtonBinding(viewModel);
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
            ProductsGrid.SetBinding(ProductDetailsGrid.ItemsSourceProperty, new Binding
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
            
            SetBindingsForDatePickers("DeliveryDate", DeliveryDatePicker);

            SetBindingsForTextBox("TruckLicensePlate", LicensePlate);

            SetBindingsForNumericTextBox("Rental", Rental);

            SetBindingsForComboBox("DeliveryEx", DeliveryEx);

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
            TermsOfPayment.DataProvider = provider;

            var binding = new Binding("TermsOfPayment")
            {
                UpdateSourceTrigger = UpdateSourceTrigger.Default,
                Mode = BindingMode.TwoWay
            };
            TermsOfPayment.SetBinding(AutoCompleteBox.TextProperty, binding);
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
            SetButtonBinding(viewModel, SaveButton, "SaveAll", viewModel.SaveAll);
            SaveButton.SetBinding(IsEnabledProperty, new Binding("SaveButtonEnabled"));
        }

        private void SetReloadButtonBinding(SaleConfirmationViewModel viewModel)
        {
            SetButtonBinding(viewModel, UndoButton, "Reload", viewModel.Reload);
            UndoButton.SetBinding(IsEnabledProperty, new Binding("ReloadButtonEnabled"));
        }

        private void SetPrintButtonBinding(SaleConfirmationViewModel viewModel)
        {
            SetButtonBinding(viewModel, PdfButton, "Print", viewModel.Print);
            SetButtonBinding(viewModel, ProviderPdfButton, "PrintForProvider", viewModel.PrintForProvider);
            SetButtonBinding(viewModel, CustomerPdfButton, "PrintForCustomer", viewModel.PrintForCustomer);
        }

        private void SetConvertButtonBinding(SaleConfirmationViewModel viewModel)
        {
            SetButtonBinding(viewModel, ConvertButton, "Convert", viewModel.Convert);
            ConvertButton.SetBinding(IsEnabledProperty, new Binding("ActionButtonsEnabled"));
        }

        private void SetSendtButtonBinding(SaleConfirmationViewModel viewModel)
        {
            //SetMenuItemBinding(viewModel, EmailButton, "Email", viewModel.Email);
            SetMenuItemBinding(viewModel, ProviderEmailButton, "EmailToProvider", viewModel.EmailToProvider);
            SetMenuItemBinding(viewModel, CustomerEmailButton, "EmailToCustomer", viewModel.EmailToCustomer);
            SetMenuItemBinding(viewModel, TransportEmailButton, "EmailToTransporter", viewModel.EmailToTransporter);
        }

        private void SetCloneButtonBinding(SaleConfirmationViewModel viewModel)
        {
            SetButtonBinding(viewModel, CloneButton, "Clone", viewModel.Clone);
            CloneButton.SetBinding(IsEnabledProperty, new Binding("ActionButtonsEnabled"));
        }

        private void SetAttachmentButtonBinding(SaleConfirmationViewModel viewModel)
        {
            SetButtonBinding(viewModel, OpenAttachmentsButton, "OpenAttachments", viewModel.OpenAttachments);
            OpenAttachmentsButton.SetBinding(IsEnabledProperty, new Binding("OpenAttachmentsButtonEnabled"));
        }
    }
}
