﻿using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Models.Companies;
using PosizioniRoverfrutta.Services;
using PosizioniRoverfrutta.ViewModels;
using QueryManager;

namespace PosizioniRoverfrutta.Windows
{
    /// <summary>
    /// Interaction logic for InvoicesList.xaml
    /// </summary>
    public partial class InvoicesList : BaseWindow
    {
        public InvoicesList()
            : this(null, null)
        {

        }

        public InvoicesList(IWindowManager windowManager, IDataStorage dataStorage, string documentId)
            : this(windowManager, dataStorage)
        {

        }

        public InvoicesList(IWindowManager windowManager, IDataStorage dataStorage)
            :base(windowManager, dataStorage)
        {
            InitializeComponent();
            var viewModel = new SummaryAndInvoiceViewModel(dataStorage, WindowManager);
            DataContext = viewModel;

            var companyDataProvider = new CustomerNamesAutoCompleteBoxProvider<Customer>(dataStorage);
            CompanyNameBox.DataProvider = companyDataProvider;

            var companyNameBinding = new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("CustomerName"),
                UpdateSourceTrigger = UpdateSourceTrigger.Default,
                Mode = BindingMode.OneWayToSource
            };
            CompanyNameBox.SetBinding(AutoCompleteBox.TextProperty, companyNameBinding);

            SetBindingsForDatePickers("StartDate", FromDatePicker);
            SetBindingsForDatePickers("EndDate", ToDatePicker);
            SetCheckboxBinding(ShowOpenPositionsCheckBox, "IncludeOpenPositions");
            SetBindingsForPriceTotals("CommissionsTotal", CommissionsBlock);
            SetBindingsForNumericTextBox("InvoiceVat", InvoiceVatTextBox);
            SetBindingsForPriceTotals("CalculatedInvoiceVat", CalculatedInvoiceVatTextBox);
            SetBindingsForNumericTextBox("Witholding", WitholdingTextBox);
            SetBindingsForPriceTotals("TaxedAmount", TaxedAmountTextBox);
            SetBindingsForPriceTotals("CalculatedWitholding", WitholdingTextBlock);
            SetBindingsForPriceTotals("NetAmount", NetAmountTextBlock);
            SetBindingsForNumericTextBox("InvoiceNumber", InvoiceNumberBox);
            SetBindingsForDatePickers("InvoiceDate", InvoiceDatePicker);
            SetSaveButtonBinding(viewModel);
            SetPrintSummaryButtonBinding(viewModel);
            SetPrintInvoicButtonBinding(viewModel);
            SetStatusBinding();

            BuildSummaryDataGridColumns();
            SetSummaryDataGridBinding(viewModel);

            BuildPartialsDataGridColumns();
            SetPartialsDataGridBinding(viewModel);

            SetVatVisibility(viewModel);
        }

        private void SetSaveButtonBinding(SummaryAndInvoiceViewModel viewModel)
        {
            SetButtonBinding(viewModel, SaveButton, "Save", viewModel.Save);
            SaveButton.SetBinding(IsEnabledProperty, new Binding("SaveButtonEnabled"));
        }

        private void BuildPartialsDataGridColumns()
        {
            var companyColumn = BuildReadOnlyTextColumn("Cliente/Fornitore", "CompanyName", 3);
            PartialsDataGrid.Columns.Add(companyColumn);
            var totalColumn = BuildReadOnlyDecimalColumn("Totale Commissioni", "Total", 1);
            PartialsDataGrid.Columns.Add(totalColumn);
        }

        private void BuildSummaryDataGridColumns()
        {
            var idColumn = BuildReadOnlyTextColumn("Posizione", "DocumentId", 1);
            SummaryDataGrid.Columns.Add(idColumn);
            var dateColumn = BuildReadOnlyDateColumn("Data Spedizione", "ShippingDate", 2);
            SummaryDataGrid.Columns.Add(dateColumn);
            var ddtColumn = BuildReadOnlyTextColumn("D.D.T.", "TransportDocument", 2);
            SummaryDataGrid.Columns.Add(ddtColumn);
            var companyColumn = BuildReadOnlyTextColumn("Cliente/Fornitore", "CompanyName", 2.5);
            SummaryDataGrid.Columns.Add(companyColumn);
            var taxableColumn = BuildReadOnlyDecimalColumn("Imponibile EURO", "TaxableAmount", 1.6);
            SummaryDataGrid.Columns.Add(taxableColumn);
            var commissionColumn = BuildReadOnlyDecimalColumn("Commissione %", "Commission", 1.5);
            SummaryDataGrid.Columns.Add(commissionColumn);
            var payableColumn = BuildReadOnlyDecimalColumn("Provvigione", "PayableAmount", 1.5);
            SummaryDataGrid.Columns.Add(payableColumn);
            var includeInInvoiceColumn = BuildCheckBoxColumn("Incl. in Fattura", "CanMakeInvoice", 1);
            SummaryDataGrid.Columns.Add(includeInInvoiceColumn);
        }

        private static DataGridTextColumn BuildReadOnlyTextColumn(string header, string propertyName, double size)
        {
            return new DataGridTextColumn
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
        }

        private static DataGridTextColumn BuildReadOnlyDateColumn(string header, string propertyName, double size)
        {
            return new DataGridTextColumn
            {
                Header = header,
                IsReadOnly = true,
                Binding = new Binding(propertyName)
                {
                    Mode = BindingMode.Default,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    StringFormat = "d",
                    ConverterCulture = CultureInfo.CurrentCulture
                },
                Width = new DataGridLength(size, DataGridLengthUnitType.Star),
            };
        }

        private static DataGridTextColumn BuildReadOnlyDecimalColumn(string header, string propertyName, double size)
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
                Width = new DataGridLength(size, DataGridLengthUnitType.Star),
            };
        }

        private static DataGridCheckBoxColumn BuildCheckBoxColumn(string header, string propertyName, double size)
        {
            return new DataGridCheckBoxColumn
            {
                Header = header,
                IsReadOnly = false,
                Binding = new Binding(propertyName)
                {
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                },
                Width = new DataGridLength(size, DataGridLengthUnitType.Star),
            };
        }

        private void SetSummaryDataGridBinding(SummaryAndInvoiceViewModel viewModel)
        {
            SummaryDataGrid.SetBinding(DataGrid.ItemsSourceProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("SummaryRows"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });
        }

        private void SetPartialsDataGridBinding(SummaryAndInvoiceViewModel viewModel)
        {
            PartialsDataGrid.SetBinding(DataGrid.ItemsSourceProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("PartialsByCompanyName"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
        }

        private void SetPrintSummaryButtonBinding(SummaryAndInvoiceViewModel viewModel)
        {
            SetButtonBinding(viewModel, SummaryPdfButton, "PrintSummary", viewModel.PrintSummary);
        }

        private void SetPrintInvoicButtonBinding(SummaryAndInvoiceViewModel viewModel)
        {
            SetButtonBinding(viewModel, InvoicePdfButton, "PrintInvoice", viewModel.PrintInvoice);
        }

        private void SetCheckboxBinding(CheckBox checkBoxControl, string propertyName)
        {
            var myBinding = new Binding(propertyName)
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            checkBoxControl.SetBinding(ToggleButton.IsCheckedProperty, myBinding);
        }

        private void SetVatVisibility(SummaryAndInvoiceViewModel viewModel)
        {
            var visibilityBinding = new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("ShowVatArea"),
                Converter = (IValueConverter)FindResource("visibilityConverter"),
            };
            VatPanel.SetBinding(VisibilityProperty, visibilityBinding);
            CalculatedInvoiceVatTextBox.SetBinding(VisibilityProperty, visibilityBinding);
            WitholdingPanel.SetBinding(VisibilityProperty, visibilityBinding);
            WitholdingTextBlock.SetBinding(VisibilityProperty, visibilityBinding);
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
    }
}
