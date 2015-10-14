using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using QueryManager;
using PosizioniRoverfrutta.ViewModels;
using System.Windows.Controls.Primitives;
using System.Globalization;
using Models.Entities;

namespace PosizioniRoverfrutta.Windows
{
    /// <summary>
    /// Interaction logic for CustomersWindow.xaml
    /// </summary>
    public partial class CustomersWindow : BaseWindow
    {
        public CustomersWindow()
            : this(null, null)
        {
            
        }

        public CustomersWindow(IWindowManager windowManager, IDataStorage dataStorage, string documentId)
            : this(windowManager, dataStorage)
        {
            
        }

        public CustomersWindow(IWindowManager windowManager, IDataStorage dataStorage) : base(windowManager, dataStorage)
        {
            InitializeComponent();
            var viewModel = new CustomersWindowGridViewModel(dataStorage, windowManager);
            this.DataContext = viewModel;

            SetBindingsForTextBox("SearchBox", SearchBox);
            SetBindingsAndStatusForTextBox("CompanyName", CompanyNameBox);
            SetBindingsAndStatusForTextBox("Address", Address);
            SetBindingsAndStatusForTextBox("City", City);
            SetBindingsAndStatusForTextBox("StateOrProvince", County);
            SetBindingsAndStatusForTextBox("PostCode", PostalCode);
            SetBindingsAndStatusForTextBox("Country", Country);
            SetBindingsAndStatusForTextBox("VatCode", VatCode);
            SetBindingsAndStatusForTextBox("EmailAddress", EmailAddress);
            SetBindingsForCheckBox("DoNotApplyVat", DoNotApplyVatCheckBox);
            DoNotApplyVatCheckBox.SetBinding(IsEnabledProperty, new Binding("EditControlsEnabled"));

            SetAddCustomerButtonBinding(viewModel);
            SetSaveButtonBindings(viewModel);
            SetDeleteButtonBindings(viewModel);
            SetPreviousPageButtonBindings(viewModel);
            SetNextPageButtonBindings(viewModel);

            BuildDataGridColumns();

            SetDataGridBinding(viewModel);

            this.Activated += CustomersWindow_Activated;
            CustomersGrid.SelectionChanged += CustomersGrid_SelectionChanged;
        }

        private void CustomersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedId = null;
            if (CustomersGrid.SelectedItem != null)
            {
                var customerRow = (CustomerRow)CustomersGrid.SelectedItem;
                selectedId = customerRow.Id;
            }
            ((CustomersWindowGridViewModel)DataContext).LoadSelectedCustomer(selectedId);
        }

        private void SetBindingsAndStatusForTextBox(string property, TextBox control)
        {
            SetBindingsForTextBox(property, control);
            control.SetBinding(IsEnabledProperty, new Binding("EditControlsEnabled"));
        }

        private void BuildDataGridColumns()
        {
            var idColumn = BuildReadOnlyTextColumn("Id", "Id", 20, true);
            CustomersGrid.Columns.Add(idColumn);
            var nameColumn = BuildReadOnlyTextColumn("Nome", "CompanyName", 40);
            CustomersGrid.Columns.Add(nameColumn);
            var salesConfirmationsColumn = BuildReadOnlyTextColumn("N° Conf.Vendita", "NumberOfSalesConfirmations", 20);
            CustomersGrid.Columns.Add(salesConfirmationsColumn);
            var loadingDocumentsColumn = BuildReadOnlyTextColumn("N° Dist.Carico", "NumberOfLoadingDocuments", 20);
            CustomersGrid.Columns.Add(loadingDocumentsColumn);
            var priceConfirmationsColumn = BuildReadOnlyTextColumn("N° Conf.Prezzi", "NumberOfPriceConfirmations", 20);
            CustomersGrid.Columns.Add(priceConfirmationsColumn);
        }

        private void SetDeleteButtonBindings(CustomersWindowGridViewModel viewModel)
        {
            SetButtonBinding(viewModel, DeleteButton, "DeleteCustomer", viewModel.DeleteCustomer);
            DeleteButton.SetBinding(IsEnabledProperty, new Binding("DeleteButtonEnabled"));
        }

        private void SetSaveButtonBindings(CustomersWindowGridViewModel viewModel)
        {
            SetButtonBinding(viewModel, SaveButton, "Save", viewModel.Save);
            SaveButton.SetBinding(IsEnabledProperty, new Binding("SaveButtonEnabled"));
        }

        private void SetAddCustomerButtonBinding(CustomersWindowGridViewModel viewModel)
        {
            SetButtonBinding(viewModel, AddButton, "CreateNew", viewModel.CreateNew);
            AddButton.Click += AddButton_Click;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            CustomersGrid.UnselectAll();
        }

        private void SetPreviousPageButtonBindings(CustomersWindowGridViewModel viewModel)
        {
            SetButtonBinding(viewModel, PreviousPageButton, "PreviousPage", viewModel.PreviousPage);
        }

        private void SetNextPageButtonBindings(CustomersWindowGridViewModel viewModel)
        {
            SetButtonBinding(viewModel, NextPageButton, "NextPage", viewModel.NextPage);
        }

        private void SetBindingsForCheckBox(string propertyName, CheckBox checkBoxControl)
        {
            var myBinding = new Binding(propertyName)
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            checkBoxControl.SetBinding(ToggleButton.IsCheckedProperty, myBinding);
        }

        private void SetDataGridBinding(CustomersWindowGridViewModel viewModel)
        {
            CustomersGrid.SetBinding(ItemsControl.ItemsSourceProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("CustomersList"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });
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

        private void CustomersWindow_Activated(object sender, EventArgs e)
        {
            ((CustomersWindowGridViewModel)DataContext).Refresh.Execute(null);
        }
    }
}
