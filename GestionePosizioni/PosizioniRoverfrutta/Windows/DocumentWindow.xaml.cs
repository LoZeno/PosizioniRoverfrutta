using System.Windows;
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

            var companyDetailsControl = new CompanyDetails(dataStorage, viewModel.CustomerControlViewModel);
            companyDetailsControl.MinWidth = 300;
            MyPanel.Children.Add(companyDetailsControl);

            DataContext = viewModel;

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

        public override int Index { get; set; }
    }
}
