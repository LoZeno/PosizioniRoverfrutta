using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using PosizioniRoverfrutta.CustomControls.DataGridColumns;
using PosizioniRoverfrutta.Services;

namespace PosizioniRoverfrutta.CustomControls
{
    /// <summary>
    /// Interaction logic for ProductDetailsGrid.xaml
    /// </summary>
    public partial class ProductDetailsGrid : UserControl
    {
        public ProductDetailsGrid()
        {
            InitializeComponent();

            BuildDataGridColumns();
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
          DataGrid.ItemsSourceProperty.AddOwner(typeof(ProductDetailsGrid));

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

            var palletTypeColumn = BuildPalletTypesDataGridColumn();
            BuildComboBoxColumn("Tipo Pal", "PalletType");
            ProductsGrid.Columns.Add(palletTypeColumn);

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
                Mode = BindingMode.OneWay,
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
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
            };

            var autocompleteBoxFactory = new FrameworkElementFactory(typeof(ProductDescriptionAutocompleteBoxWrapper));
            autocompleteBoxFactory.SetBinding(AutoCompleteBox.TextProperty, autocompleteBinding);

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
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
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
            autocompleteBoxFactory.SetBinding(AutoCompleteBox.TextProperty, autocompleteBinding);

            var autocompleteTemplate = new DataTemplate();
            autocompleteTemplate.VisualTree = autocompleteBoxFactory;

            // Set the Templates to the Column
            templateColumn.CellTemplate = textTemplate;
            templateColumn.CellEditingTemplate = autocompleteTemplate;

            return templateColumn;
        }

        private DataGridTemplateColumn BuildPalletTypesDataGridColumn()
        {
            // Create The Column
            var templateColumn = new DataGridTemplateColumn
            {
                Header = "Tipo Pal",
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            };

            var binding = new Binding("PalletType")
            {
                Mode = BindingMode.OneWay
            };

            // Create the TextBlock
            var textFactory = new FrameworkElementFactory(typeof(TextBlock));
            textFactory.SetBinding(TextBlock.TextProperty, binding);
            var textTemplate = new DataTemplate();
            textTemplate.VisualTree = textFactory;

            // Create the ComboBox
            var autocompleteBinding = new Binding("PalletType")
            {
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.Default
            };

            var autocompleteBoxFactory = new FrameworkElementFactory(typeof(PalletTypesComboBoxWrapper));
            autocompleteBoxFactory.SetBinding(ComboBox.TextProperty, autocompleteBinding);

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

        private static DataGridColumn BuildComboBoxColumn(string header, string propertyName)
        {
            var binding = new Binding(propertyName)
            {
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };

            var comboBoxStyle = new Style(typeof(ComboBox));
            comboBoxStyle.Setters.Add(new Setter(ComboBox.IsEditableProperty, true));
            return new DataGridComboBoxColumn
            {
                ItemsSource = new PalletTypesComboBoxProvider().GetItems(),
                Header = header,
                SelectedValueBinding = binding,
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                EditingElementStyle = comboBoxStyle
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
    }
}
