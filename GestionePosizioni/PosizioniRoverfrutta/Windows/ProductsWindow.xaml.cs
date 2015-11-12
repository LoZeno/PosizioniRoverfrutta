using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Models.Entities;
using PosizioniRoverfrutta.ViewModels;
using QueryManager;

namespace PosizioniRoverfrutta.Windows
{
    /// <summary>
    /// Interaction logic for ProductsWindow.xaml
    /// </summary>
    public partial class ProductsWindow : BaseWindow
    {
        public ProductsWindow()
            : this(null, null)
        {
            
        }

        public ProductsWindow(IWindowManager windowManager, IDataStorage dataStorage, string documentId)
            : this(windowManager, dataStorage)
        {
            
        }

        public ProductsWindow(IWindowManager windowManager, IDataStorage dataStorage) : base(windowManager, dataStorage)
        {
            InitializeComponent();
            var viewModel = new ProductsWindowGridViewModel(dataStorage, windowManager);
            this.DataContext = viewModel;

            SetBindingsForTextBox("SearchBox", SearchBox);
            SetBindingsAndStatusForTextBox("Description", DescriptionBox);

            SetAddProductButtonBinding(viewModel);
            SetSaveButtonBindings(viewModel);
            SetDeleteButtonBindings(viewModel);
            SetPreviousPageButtonBindings(viewModel);
            SetNextPageButtonBindings(viewModel);

            BuildDataGridColumns();

            SetDataGridBinding(viewModel);

            this.Activated += ProductsWindow_Activated; ;
            ProductsGrid.SelectionChanged += ProductsGrid_SelectionChanged; ;
        }

        private void ProductsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int? selectedId = null;
            if (ProductsGrid.SelectedItem != null)
            {
                var customerRow = (ProductRow)ProductsGrid.SelectedItem;
                var originalId = customerRow.Id.Split('/');
                if (originalId.Count() > 1)
                {
                    selectedId = int.Parse(originalId[1]);
                }
                else
                {
                    selectedId = int.Parse(originalId[0]);
                }
            }
            ((ProductsWindowGridViewModel)DataContext).LoadSelectedProduct(selectedId);
        }

        private void ProductsWindow_Activated(object sender, EventArgs e)
        {
            ((ProductsWindowGridViewModel)DataContext).Refresh.Execute(null);

            if (WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
            }
        }

        private void SetBindingsAndStatusForTextBox(string property, TextBox control)
        {
            SetBindingsForTextBox(property, control);
            control.SetBinding(IsEnabledProperty, new Binding("EditControlsEnabled"));
        }

        private void BuildDataGridColumns()
        {
            var idColumn = BuildReadOnlyTextColumn("Id", "Id", 20, true);
            ProductsGrid.Columns.Add(idColumn);
            var descriptionColumn = BuildReadOnlyTextColumn("Descrizione", "Description", 40);
            ProductsGrid.Columns.Add(descriptionColumn);
            var salesConfirmationsColumn = BuildReadOnlyTextColumn("N° Conf.Vendita", "NumberOfSalesConfirmations", 20);
            ProductsGrid.Columns.Add(salesConfirmationsColumn);
            var loadingDocumentsColumn = BuildReadOnlyTextColumn("N° Dist.Carico", "NumberOfLoadingDocuments", 20);
            ProductsGrid.Columns.Add(loadingDocumentsColumn);
            var priceConfirmationsColumn = BuildReadOnlyTextColumn("N° Conf.Prezzi", "NumberOfPriceConfirmations", 20);
            ProductsGrid.Columns.Add(priceConfirmationsColumn);
        }

        private void SetDeleteButtonBindings(ProductsWindowGridViewModel viewModel)
        {
            SetButtonBinding(viewModel, DeleteButton, "DeleteProduct", viewModel.DeleteProduct);
            DeleteButton.SetBinding(IsEnabledProperty, new Binding("DeleteButtonEnabled"));
        }

        private void SetSaveButtonBindings(ProductsWindowGridViewModel viewModel)
        {
            SetButtonBinding(viewModel, SaveButton, "Save", viewModel.Save);
            SaveButton.SetBinding(IsEnabledProperty, new Binding("SaveButtonEnabled"));
        }

        private void SetAddProductButtonBinding(ProductsWindowGridViewModel viewModel)
        {
            SetButtonBinding(viewModel, AddButton, "CreateNew", viewModel.CreateNew);
            AddButton.Click += AddButton_Click;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ProductsGrid.UnselectAll();
        }

        private void SetPreviousPageButtonBindings(ProductsWindowGridViewModel viewModel)
        {
            SetButtonBinding(viewModel, PreviousPageButton, "PreviousPage", viewModel.PreviousPage);
        }

        private void SetNextPageButtonBindings(ProductsWindowGridViewModel viewModel)
        {
            SetButtonBinding(viewModel, NextPageButton, "NextPage", viewModel.NextPage);
        }

        private void SetDataGridBinding(ProductsWindowGridViewModel viewModel)
        {
            ProductsGrid.SetBinding(ItemsControl.ItemsSourceProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("ProductsList"),
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
    }
}
