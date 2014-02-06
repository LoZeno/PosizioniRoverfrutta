using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using GestionePosizioni.BaseClasses;
using GestionePosizioni.CustomControls;
using GestionePosizioni.CustomControls.ControlServices;
using GestionePosizioni.ViewModels;
using QueryManager.Repositories;

namespace GestionePosizioni
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : BaseWindow
    {
        private readonly CustomerDetailsViewModel _cvm;
        private int _errors = 0;
        private CompanyDetails _providerControl;

        private readonly IMainViewModel _windowViewModel;

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(IMainViewModel viewModel)
            : this()
        {
            _windowViewModel = viewModel;
            DataContext = viewModel;
            var saveBinding = new CommandBinding
            {
                Command = viewModel.Save,
            };
            saveBinding.CanExecute += OnCanExecute;
            saveBinding.Executed += saveBinding_Executed;
            CommandBindings.Add(saveBinding);
            SetBindings();
            var customerRepo = new CustomerRepository(DatabaseSession);
            var dataProviderForCustomer = new CustomerAutoCompleteBoxProvider(customerRepo);
            _cvm = new CustomerDetailsViewModel(_windowViewModel.Customer, customerRepo);
            _providerControl = new CompanyDetails(_cvm, dataProviderForCustomer);
            Grid.SetColumn(_providerControl, 0);
            Grid.SetRow(_providerControl, 1);
            ContentGrid.Children.Add(_providerControl);
        }

        void saveBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            StatusLabel.Content = "Salvataggio eseguito ";
            e.Handled = true;
        }

        private void OnCanExecute(object sender, CanExecuteRoutedEventArgs canExecuteRoutedEventArgs)
        {
            canExecuteRoutedEventArgs.CanExecute = _errors == 0;
            canExecuteRoutedEventArgs.Handled = true;
        }

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                _errors++;
            else
                _errors--;
        }

        private void SetBindings()
        {
            SaveButton.SetBinding(ButtonBase.CommandProperty, new Binding
            {
                Source = _windowViewModel,
                Path = new PropertyPath("Save")
            });

            PositionNumberTextBox.SetBinding(TextBox.TextProperty, new Binding
            {
                Source = _windowViewModel,
                Path = new PropertyPath("DocumentId"),
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus,
                Mode = BindingMode.TwoWay
            });

            //_providerControl.SetBinding(CompanyDetails.SelectedCompanyProperty, new Binding
            //{
            //    Source = _windowViewModel,
            //    Path = new PropertyPath("Provider"),
            //    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            //    Mode = BindingMode.TwoWay
            //});

            ProductsGrid.SetBinding(DataGrid.ItemsSourceProperty, new Binding
            {
                Source = _windowViewModel,
                Path = new PropertyPath("Products"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });
            
            BuildProductGridColumns();
        }

        private void BuildProductGridColumns()
        {
            var productDescriptionColumn = new DataGridTextColumn
            {
                Header = "Prodotto",
                Binding = new Binding("ProductDescription")
                {
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                },
                Width = new DataGridLength(4, DataGridLengthUnitType.Star),
            };
            ProductsGrid.Columns.Add(productDescriptionColumn);
            var palletsColumn = new DataGridTextColumn
            {
                Header = "Pallets",
                Binding = new Binding("Pallets")
                {
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                },
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
            };
            ProductsGrid.Columns.Add(palletsColumn);
            var packagesColumn = new DataGridTextColumn
            {
                Header = "Colli",
                Binding = new Binding("Packages")
                {
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                },
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
            };
            ProductsGrid.Columns.Add(packagesColumn);
            var grossWeightColumn = new DataGridTextColumn
            {
                Header = "Kg Lordo",
                Binding = new Binding("GrossWeight")
                {
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                },
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
            };
            ProductsGrid.Columns.Add(grossWeightColumn);
            var netWeightColumn = new DataGridTextColumn
            {
                Header = "Kg Netto",
                Binding = new Binding("NetWeight")
                {
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                },
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
            };
            ProductsGrid.Columns.Add(netWeightColumn);
            var priceColumn = new DataGridTextColumn
            {
                Header = "Prezzo",
                Binding = new Binding("Price")
                {
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                },
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
            };
            ProductsGrid.Columns.Add(priceColumn);
            var currencyColumn = new DataGridTextColumn
            {
                Header = "Valuta",
                Binding = new Binding("Currency")
                {
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                },
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
            };
            ProductsGrid.Columns.Add(currencyColumn);
            var priceParameterColumn = new DataGridTextColumn
            {
                Header = "Parametro",
                Binding = new Binding("PriceParameter")
                {
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                },
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
            };
            ProductsGrid.Columns.Add(priceParameterColumn);
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            _cvm.Save.Execute(null);
        }
    }
}
