using Models.Entities;
using PosizioniRoverfrutta.ViewModels;
using QueryManager;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace PosizioniRoverfrutta.Windows
{
    /// <summary>
    /// Interaction logic for TransportersWindow.xaml
    /// </summary>
    public partial class TransportersWindow : BaseWindow
    {
        public TransportersWindow()
            :this(null, null)
        {

        }
        public TransportersWindow(IWindowManager windowManager, IDataStorage dataStorage, string documentId)
            : this(windowManager, dataStorage)
        {

        }

        public TransportersWindow(IWindowManager windowManager, IDataStorage dataStorage) : base(windowManager, dataStorage)
        {
            InitializeComponent();
            var viewModel = new TransporterWindowGridViewModel(dataStorage, windowManager);
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
            TransportersGrid.SelectionChanged += CustomersGrid_SelectionChanged;
        }

        private void CustomersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedId = null;
            if (TransportersGrid.SelectedItem != null)
            {
                var customerRow = (CustomerRow)TransportersGrid.SelectedItem;
                selectedId = customerRow.Id;
            }
            ((TransporterWindowGridViewModel)DataContext).LoadSelectedTransporter(selectedId);
        }

        private void SetBindingsAndStatusForTextBox(string property, TextBox control)
        {
            SetBindingsForTextBox(property, control);
            control.SetBinding(IsEnabledProperty, new Binding("EditControlsEnabled"));
        }

        private void BuildDataGridColumns()
        {
            var idColumn = BuildReadOnlyTextColumn("Id", "Id", 20, true);
            TransportersGrid.Columns.Add(idColumn);
            var nameColumn = BuildReadOnlyTextColumn("Nome", "CompanyName", 40);
            TransportersGrid.Columns.Add(nameColumn);
            var salesConfirmationsColumn = BuildReadOnlyTextColumn("N° Conf.Vendita", "NumberOfSalesConfirmations", 20);
            TransportersGrid.Columns.Add(salesConfirmationsColumn);
            var loadingDocumentsColumn = BuildReadOnlyTextColumn("N° Dist.Carico", "NumberOfLoadingDocuments", 20);
            TransportersGrid.Columns.Add(loadingDocumentsColumn);
            var priceConfirmationsColumn = BuildReadOnlyTextColumn("N° Conf.Prezzi", "NumberOfPriceConfirmations", 20);
            TransportersGrid.Columns.Add(priceConfirmationsColumn);
        }

        private void SetDeleteButtonBindings(TransporterWindowGridViewModel viewModel)
        {
            SetButtonBinding(viewModel, DeleteButton, "DeleteTransporter", viewModel.DeleteTransporter);
            DeleteButton.SetBinding(IsEnabledProperty, new Binding("DeleteButtonEnabled"));
        }

        private void SetSaveButtonBindings(TransporterWindowGridViewModel viewModel)
        {
            SetButtonBinding(viewModel, SaveButton, "Save", viewModel.Save);
            SaveButton.SetBinding(IsEnabledProperty, new Binding("SaveButtonEnabled"));
        }

        private void SetAddCustomerButtonBinding(TransporterWindowGridViewModel viewModel)
        {
            SetButtonBinding(viewModel, AddButton, "CreateNew", viewModel.CreateNew);
            AddButton.Click += AddButton_Click;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            TransportersGrid.UnselectAll();
        }

        private void SetPreviousPageButtonBindings(TransporterWindowGridViewModel viewModel)
        {
            SetButtonBinding(viewModel, PreviousPageButton, "PreviousPage", viewModel.PreviousPage);
        }

        private void SetNextPageButtonBindings(TransporterWindowGridViewModel viewModel)
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

        private void SetDataGridBinding(TransporterWindowGridViewModel viewModel)
        {
            TransportersGrid.SetBinding(ItemsControl.ItemsSourceProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("TransportersList"),
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
            ((TransporterWindowGridViewModel)DataContext).Refresh.Execute(null);
        }
    }
}
