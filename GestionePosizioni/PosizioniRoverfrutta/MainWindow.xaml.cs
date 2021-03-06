﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Models.Companies;
using PosizioniRoverfrutta.CustomControls.DataGridColumns;
using PosizioniRoverfrutta.Services;
using PosizioniRoverfrutta.ViewModels;
using PosizioniRoverfrutta.Windows;
using QueryManager;

namespace PosizioniRoverfrutta
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly WindowManager _windowsManager;
        private readonly IDataStorage _dataStorage;

        private ListPositionsViewModel _viewModel
        {
            get { return (ListPositionsViewModel) DataContext; }
        }

        public MainWindow(WindowManager windowManager, IDataStorage dataStorage)
        {
            InitializeComponent();
            _windowsManager = windowManager;
            _dataStorage = dataStorage;
            var viewModel = new ListPositionsViewModel(_dataStorage);
            this.DataContext = viewModel;

            SetAutocompleteBoxBinding(dataStorage, viewModel);
            
            BuildDataGridColumns();
            SetDataGridBinding(viewModel);
            SetBindingsForDatePickers("FromDate", FromDatePicker);
            SetBindingsForDatePickers("ToDate", ToDatePicker);
            SetNextPageBinding(viewModel);
            SetPreviousPageBinding(viewModel);
            SetEnableButtonBinding(viewModel, OpenSaleConfirmationButton, "OpenSaleConfirmationIsEnabled");
            SetEnableButtonBinding(viewModel, OpenLoadingDocumentButton, "OpenLoadingDocumentIsEnabled");
            SetEnableButtonBinding(viewModel, OpenPriceConfirmationButton, "OpenPriceConfirmationIsEnabled");
            this.Activated += MainWindow_Activated;
        }

        private void SetAutocompleteBoxBinding(IDataStorage dataStorage, ListPositionsViewModel viewModel)
        {
            var companyDataProvider = new CustomerNamesAutoCompleteBoxProvider<Customer>(dataStorage);
            CompanyNameBox.DataProvider = companyDataProvider;

            var companyNameBinding = new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("CompanyName"),
                UpdateSourceTrigger = UpdateSourceTrigger.Default,
                Mode = BindingMode.TwoWay
            };
            CompanyNameBox.SetBinding(AutoCompleteBox.TextProperty, companyNameBinding);
        }

        private static DataGridColumn BuildNumericColumn(string header, string propertyName)
        {
            return new DataGridNumericColumn
            {
                Header = header,
                IsReadOnly = true,
                Binding = new Binding(propertyName)
                {
                    Mode = BindingMode.Default,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                },
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
            };
        }

        private static DataGridTextColumn BuildDateColumn(string header, string propertyName)
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
                Width = new DataGridLength(2, DataGridLengthUnitType.Star),
            };
        }

        private static DataGridTextColumn BuildTextColumn(string header, string propertyName)
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
                Width = new DataGridLength(3, DataGridLengthUnitType.Star),
            };
        }

        private static DataGridCheckBoxColumn BuildCheckBoxColumn(string header, string propertyName)
        {
            return new DataGridCheckBoxColumn
            {
                Header = header,
                IsReadOnly = true,
                Binding = new Binding(propertyName)
                {
                    Mode = BindingMode.Default,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                },
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
            };
        }

        private void BuildDataGridColumns()
        {
            var positionNumber = BuildNumericColumn("N° Posizione", "ProgressiveNumber");
            ListPositionsGrid.Columns.Add(positionNumber);

            var shippingDate = BuildDateColumn("Data Spedizione", "ShippingDate");
            ListPositionsGrid.Columns.Add(shippingDate);

            var customerName = BuildTextColumn("Cliente", "CustomerName");
            ListPositionsGrid.Columns.Add(customerName);

            var providerName = BuildTextColumn("Fornitore", "ProviderName");
            ListPositionsGrid.Columns.Add(providerName);

            var documentDate = BuildDateColumn("Data Documento", "DocumentDate");
            ListPositionsGrid.Columns.Add(documentDate);

            var hasLoadingDocument = BuildCheckBoxColumn("Dist. Carico", "HasLoadingDocument");
            ListPositionsGrid.Columns.Add(hasLoadingDocument);

            var hasPriceConfirmation = BuildCheckBoxColumn("Conf. Prezzi", "HasPriceConfirmation");
            ListPositionsGrid.Columns.Add(hasPriceConfirmation);
        }

        private void SetDataGridBinding(ListPositionsViewModel viewModel)
        {
            ListPositionsGrid.SetBinding(DataGrid.ItemsSourceProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("PositionsList"),
            });

            ListPositionsGrid.SetBinding(DataGrid.SelectedItemProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("SelectedPosition"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            });
        }

        private void SetEnableButtonBinding(ListPositionsViewModel viewModel, Button button, string propertyName)
        {
            button.SetBinding(Button.IsEnabledProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath(propertyName),
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

        private void SetNextPageBinding(ListPositionsViewModel viewModel)
        {
            var nextPageBinding = new CommandBinding
            {
                Command = viewModel.NextPage
            };
            CommandBindings.Add(nextPageBinding);

            NextPage.SetBinding(ButtonBase.CommandProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("NextPage")
            });
        }

        private void SetPreviousPageBinding(ListPositionsViewModel viewModel)
        {
            var previousPageBinding = new CommandBinding
            {
                Command = viewModel.NextPage
            };
            CommandBindings.Add(previousPageBinding);

            PreviousPage.SetBinding(ButtonBase.CommandProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("PreviousPage")
            });
        }

        private void MainWindow_Activated(object sender, System.EventArgs e)
        {
            ((ListPositionsViewModel)DataContext).Refresh.Execute(null);

            if (WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
            }
        }

        private void NewDocumentButton_OnClick(object sender, RoutedEventArgs e)
        {
            _windowsManager.InstantiateWindow("new", WindowTypes.ConfermaVendita);
        }

        private void SaleConfirmSearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            var documentId = SearchTextBox.Text.Trim();
            _windowsManager.InstantiateWindow(documentId, WindowTypes.ConfermaVendita);
        }

        private void SummaryButton_OnClick(object sender, RoutedEventArgs e)
        {
            _windowsManager.InstantiateWindow("", WindowTypes.Riepiloghi);
        }

        private void LoadingDocumentSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var documentId = SearchTextBox.Text.Trim();
            _windowsManager.InstantiateWindow(documentId, WindowTypes.DistintaCarico);
        }

        private void PriceConfirmationSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var documentId = SearchTextBox.Text.Trim();
            _windowsManager.InstantiateWindow(documentId, WindowTypes.ConfermaPrezzi);
        }

        private void OpenSaleConfirmationButton_OnClick(object sender, RoutedEventArgs e)
        {
            var documentId = _viewModel.SelectedPosition.ProgressiveNumber.ToString();
            _windowsManager.InstantiateWindow(documentId, WindowTypes.ConfermaVendita);
        }

        private void OpenLoadingDocumentButton_OnClick(object sender, RoutedEventArgs e)
        {
            var documentId = _viewModel.SelectedPosition.ProgressiveNumber.ToString();
            _windowsManager.InstantiateWindow(documentId, WindowTypes.DistintaCarico);
        }

        private void OpenPriceConfirmationButton_OnClick(object sender, RoutedEventArgs e)
        {
            var documentId = _viewModel.SelectedPosition.ProgressiveNumber.ToString();
            _windowsManager.InstantiateWindow(documentId, WindowTypes.ConfermaPrezzi);
        }

        private void CustomersButton_OnClick(object sender, RoutedEventArgs e)
        {
            _windowsManager.InstantiateWindow("new", WindowTypes.AnagraficaClienti);
        }

        private void TransportersButton_OnClick(object sender, RoutedEventArgs e)
        {
            _windowsManager.InstantiateWindow("new", WindowTypes.AnagraficaTrasportatori);
        }

        private void ProductsButton_OnClick(object sender, RoutedEventArgs e)
        {
            _windowsManager.InstantiateWindow("new", WindowTypes.AnagraficaProdotti);
        }

        private void BackupButton_OnClick(object sender, RoutedEventArgs e)
        {
            var path = _windowsManager.OpenSelectFolderDialog();
            if (!string.IsNullOrWhiteSpace(path))
            {
                var backupPath = Path.Combine(path, DateTime.Now.ToFileTime().ToString());
                Directory.CreateDirectory(backupPath);
                _dataStorage.StartBackup(backupPath, false);
                Process.Start(backupPath);
                _windowsManager.PopupMessage("Backup avviato", "Backup");
            }
        }
    }
}
