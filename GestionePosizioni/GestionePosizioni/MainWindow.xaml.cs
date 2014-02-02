﻿using System.Windows;
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
        private CustomerDetailsViewModel cvm;
        private int _errors = 0;

        private IMainViewModel _windowViewModel;

        public MainWindow()
        {
            InitializeComponent();
            var customerRepo = new CustomerRepository(this.DatabaseSession);
            var dataProviderForCustomer = new CustomerAutoCompleteBoxProvider(customerRepo);
            cvm = new CustomerDetailsViewModel(customerRepo);
            CompanyDetails cd = new CompanyDetails(cvm, dataProviderForCustomer);
            Grid.SetColumn(cd, 0);
            Grid.SetRow(cd, 1);
            ContentGrid.Children.Add(cd);
        }

        public MainWindow(IMainViewModel viewModel)
            : this()
        {
            _windowViewModel = viewModel;
            this.DataContext = viewModel;
            var saveBinding = new CommandBinding
            {
                Command = viewModel.Save,
            };
            saveBinding.CanExecute += OnCanExecute;
            saveBinding.Executed += saveBinding_Executed;
            CommandBindings.Add(saveBinding);
            SetBindings();
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


        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            cvm.Save.Execute(null);
        }
    }
}
