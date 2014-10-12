using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using PosizioniRoverfrutta.CustomControls;
using PosizioniRoverfrutta.ViewModels;
using QueryManager;

namespace PosizioniRoverfrutta.Windows
{
    /// <summary>
    /// Interaction logic for DocumentWindow.xaml
    /// </summary>
    public partial class DocumentWindow
    {
        public DocumentWindow(IDataStorage dataStorage) : base(dataStorage)
        {
            InitializeComponent();
            var viewModel = new SaleConfirmationViewModel(dataStorage);

            AddCompanyDetailsControls(dataStorage, viewModel);

            DataContext = viewModel;

            SetPropertiesBindings();

            SetSaveButtonBindings(viewModel);

            SetStatusBinding();
        }

        public DocumentWindow(IDataStorage dataStorage, string documentId) : this (dataStorage)
        {
            try
            {
                var myId = int.Parse(documentId);
                ((SaleConfirmationViewModel) DataContext).Id = myId;
                IdBox.IsReadOnly = true;
            }
            catch (Exception)
            {
                if (!documentId.Equals("new"))
                    StatusLabel.Content = "La conferma di vendita " + documentId + " non è stata trovata.";
            }
        }

        private void SetPropertiesBindings()
        {
            var idBinding = new Binding("Id")
            {
                UpdateSourceTrigger = UpdateSourceTrigger.Default,
                Mode = BindingMode.TwoWay
            };
            IdBox.SetBinding(TextBox.TextProperty, idBinding);
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

        private void AddCompanyDetailsControls(IDataStorage dataStorage, SaleConfirmationViewModel viewModel)
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

        private void SetSaveButtonBindings(SaleConfirmationViewModel viewModel)
        {
            var saveBinding = new CommandBinding
            {
                Command = viewModel.SaveAll
            };
            CommandBindings.Add(saveBinding);

            SaveButton.SetBinding(ButtonBase.CommandProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("SaveAll")
            });
        }
    }
}
