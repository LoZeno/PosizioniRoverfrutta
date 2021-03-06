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
    /// Interaction logic for PriceConfirmationWindow.xaml
    /// </summary>
    public partial class PriceConfirmationWindow : BaseWindow
    {
        public PriceConfirmationWindow()
            : this(null, null)
        {
            
        }

        public PriceConfirmationWindow(IWindowManager windowManager, IDataStorage dataStorage) : base(windowManager, dataStorage)
        {
            InitializeComponent();

            var viewModel = new PriceConfirmationViewModel(dataStorage, WindowManager);
            
            SetDataGridBinding(viewModel);
            
            AddCompanyDetailsControls(dataStorage, viewModel);

            DataContext = viewModel;

            SetPropertiesBindings();
            
            SetSaveButtonBindings(viewModel);

            SetReloadButtonBinding(viewModel);

            SetSendToButtonBinding(viewModel);

            SetStatusBinding();

            SetPrintButtonBinding(viewModel);

            SetVatVisibility(viewModel);

            SetAttachmentButtonBinding(viewModel);
        }

        public PriceConfirmationWindow(IWindowManager windowManager, IDataStorage dataStorage, string documentId)
            : this(windowManager, dataStorage)
        {
            try
            {
                var myId = int.Parse(documentId);
                ((PriceConfirmationViewModel) DataContext).Id = myId;
                IdBox.IsReadOnly = true;
            }
            catch (Exception)
            {
                if (!documentId.Equals("new"))
                    StatusLabel.Content = "La conferma prezzi " + documentId + " non è stata trovata.";
            }
        }

        private void SetDataGridBinding(PriceConfirmationViewModel viewModel)
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

            SetBindingsForPriceTotals("TotalAmount", TotalAmountBlock);

            SetBindingsForTotals("InvoiceDiscount", DiscountTextBlock);

            SetBindingsForPriceTotals("CalculatedDiscount", CalculatedDiscountBlock);

            SetBindingsForPriceTotals("TaxableAmount", TaxableAmountBlock);

            SetBindingsForTextBox("Vat", VatBox);

            SetBindingsForPriceTotals("CalculatedVat", CalculatedVatBlock);

            SetBindingsForPriceTotals("FinalTotal", FinalTotalBlock);
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

        private void AddCompanyDetailsControls(IDataStorage dataStorage, PriceConfirmationViewModel viewModel)
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

        private void SetSaveButtonBindings(PriceConfirmationViewModel viewModel)
        {
            SetButtonBinding(viewModel, SaveButton, "SaveAll", viewModel.SaveAll);

            SaveButton.SetBinding(IsEnabledProperty, new Binding
            {
                Path = new PropertyPath("SaveButtonEnabled")
            });
        }

        private void SetVatVisibility(PriceConfirmationViewModel viewModel)
        {
            var visibilityBinding = new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("ShowVatArea"),
                Converter = (IValueConverter)FindResource("visibilityConverter"),
            };
            VatPanel.SetBinding(VisibilityProperty, visibilityBinding);
            CalculatedVatBlock.SetBinding(VisibilityProperty, visibilityBinding);
        }

        private void SetReloadButtonBinding(PriceConfirmationViewModel viewModel)
        {
            SetButtonBinding(viewModel, UndoButton, "Reload", viewModel.Reload);

            UndoButton.SetBinding(IsEnabledProperty, new Binding
            {
                Path = new PropertyPath("ReloadButtonEnabled")
            });
        }

        private void SetPrintButtonBinding(PriceConfirmationViewModel viewModel)
        {
            SetButtonBinding(viewModel, PdfButton, "Print", viewModel.Print);
            SetButtonBinding(viewModel, ProviderPdfButton, "PrintForProvider", viewModel.PrintForProvider);
            SetButtonBinding(viewModel, CustomerPdfButton, "PrintForCustomer", viewModel.PrintForCustomer);
        }

        private void SetSendToButtonBinding(PriceConfirmationViewModel viewModel)
        {
            SetButtonBinding(viewModel, EmailButton, "Email", viewModel.Email);
            SetButtonBinding(viewModel, ProviderEmailButton, "EmailToProvider", viewModel.EmailToProvider);
            SetButtonBinding(viewModel, CustomerEmailButton, "EmailToCustomer", viewModel.EmailToCustomer);
        }


        private void SetAttachmentButtonBinding(PriceConfirmationViewModel viewModel)
        {
            SetButtonBinding(viewModel, OpenAttachmentsButton, "OpenAttachments", viewModel.OpenAttachments);
        }
    }
}
