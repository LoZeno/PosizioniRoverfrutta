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

            SetSaveButtonBindings(viewModel);
        }

        public override int Index { get; set; }

        private void AddCompanyDetailsControls(IDataStorage dataStorage, SaleConfirmationViewModel viewModel)
        {
            var customerDetailsControl = new CompanyDetails(dataStorage, viewModel.CustomerControlViewModel);
            AddControlToGrid(customerDetailsControl, 0, 1);

            var providerDetailsControl = new CompanyDetails(dataStorage, viewModel.ProviderControlViewModel);
            AddControlToGrid(providerDetailsControl, 1, 1);
        }

        private void AddControlToGrid(CompanyDetails control, int column, int row)
        {
            Grid.SetColumn(control, column);
            Grid.SetRow(control, row);
            ContentGrid.Children.Add(control);
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
