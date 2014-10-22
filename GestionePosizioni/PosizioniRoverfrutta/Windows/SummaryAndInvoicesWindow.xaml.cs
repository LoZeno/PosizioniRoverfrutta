using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Models.Companies;
using PosizioniRoverfrutta.Services;
using PosizioniRoverfrutta.ViewModels;
using QueryManager;

namespace PosizioniRoverfrutta.Windows
{
    /// <summary>
    /// Interaction logic for SummaryAndInvoicesWindow.xaml
    /// </summary>
    public partial class SummaryAndInvoicesWindow : BaseWindow
    {
        public SummaryAndInvoicesWindow()
            : this(null, null)
        {
            
        }

        public SummaryAndInvoicesWindow(IWindowManager windowManager, IDataStorage dataStorage, string documentId)
            : this(windowManager, dataStorage)
        {
            
        }

        public SummaryAndInvoicesWindow(IWindowManager windowManager, IDataStorage dataStorage)
            :base(windowManager, dataStorage)
        {
            InitializeComponent();

            var companyDataProvider = new CustomerNamesAutoCompleteBoxProvider<Customer>(dataStorage);
            CompanyNameBox.AutoCompleteManager.DataProvider = companyDataProvider;
            CompanyNameBox.AutoCompleteManager.Asynchronous = true;

            var viewModel = new SummaryAndInvoiceViewModel(dataStorage);
            DataContext = viewModel;

            var companyNameBinding = new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("CustomerName"),
                UpdateSourceTrigger = UpdateSourceTrigger.Default,
                Mode = BindingMode.TwoWay
            };
            CompanyNameBox.SetBinding(TextBox.TextProperty, companyNameBinding);

            SetBindingsForDatePickers("StartDate", FromDatePicker);
            SetBindingsForDatePickers("EndDate", ToDatePicker);
            SetBindingsForTotals("CommissionsTotal", CommissionsBlock);
            SetBindingsForNumericTextBox("InvoiceVat", InvoiceVatTextBox);
            SetBindingsForTotals("CalculatedInvoiceVat", CalculatedInvoiceVatTextBox);
            SetBindingsForTotals("TaxedAmount", TaxedAmountTextBox);
            SetBindingsForNumericTextBox("Witholding", WitholdingTextBox);
            SetBindingsForTotals("CalculatedWitholding", WitholdingTextBlock);
            SetBindingsForTotals("NetAmount", NetAmountTextBlock);

            BuildDataGridColumns();

            SetDataGridBinding(viewModel);
        }

        private void BuildDataGridColumns()
        {
            var idColumn = BuildReadOnlyTextColumn("Posizione", "DocumentId", 1);
            SummaryDataGrid.Columns.Add(idColumn);
            var dateColumn = BuildReadOnlyTextColumn("Data", "DocumentDate", 2);
            SummaryDataGrid.Columns.Add(dateColumn);
            var ddtColumn = BuildReadOnlyTextColumn("D.D.T.", "TransportDocument", 2);
            SummaryDataGrid.Columns.Add(ddtColumn);
            var companyColumn = BuildReadOnlyTextColumn("Cliente/Fornitore", "CompanyName", 2.5);
            SummaryDataGrid.Columns.Add(companyColumn);
            var taxableColumn = BuildReadOnlyTextColumn("Imponibile EURO", "TaxableAmount", 1.6);
            SummaryDataGrid.Columns.Add(taxableColumn);
            var commissionColumn = BuildReadOnlyTextColumn("Commissione %", "Commission", 1.5);
            SummaryDataGrid.Columns.Add(commissionColumn);
            var payableColumn = BuildReadOnlyTextColumn("Provvigione", "PayableAmount", 1.5);
            SummaryDataGrid.Columns.Add(payableColumn);
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

        private void SetDataGridBinding(SummaryAndInvoiceViewModel viewModel)
        {
            SummaryDataGrid.SetBinding(DataGrid.ItemsSourceProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("SummaryRows"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });
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
    }
}
