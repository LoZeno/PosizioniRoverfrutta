using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using dragonz.actb.control;
using Models;
using Models.Companies;
using PosizioniRoverfrutta.Services;
using PosizioniRoverfrutta.ViewModels;
using QueryManager;

namespace PosizioniRoverfrutta.CustomControls
{
    /// <summary>
    /// Interaction logic for TransporterDetails.xaml
    /// </summary>
    public partial class TransporterDetails : UserControl
    {
        public TransporterDetails()
        {
            InitializeComponent();
        }

        public TransporterDetails(IDataStorage dataStorage, CompanyControlViewModel<Transporter> viewModel)
            : this()
        {
            var companyDataProvider = new CustomerNamesAutoCompleteBoxProvider<Transporter>(dataStorage);
            CompanyNameBox.AutoCompleteManager.DataProvider = companyDataProvider;
            CompanyNameBox.AutoCompleteManager.Asynchronous = true;
            DataContext = viewModel;

            var companyNameBinding = new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("CompanyName"),
                UpdateSourceTrigger = UpdateSourceTrigger.Default,
                Mode = BindingMode.TwoWay
            };
            CompanyNameBox.SetBinding(TextBox.TextProperty, companyNameBinding);

            SetTextboxBinding(Address, "Address");
            SetTextboxBinding(City, "City");
            SetTextboxBinding(County, "StateOrProvince");
            SetTextboxBinding(PostalCode, "PostCode");
            SetTextboxBinding(Country, "Country");
            SetTextboxBinding(VatCode, "VatCode");
            SetTextboxBinding(EmailAddress, "EmailAddress");
        }

        private void SetTextboxBinding(TextBox control, string propertyName)
        {
            var myBinding = new Binding(propertyName)
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            control.SetBinding(TextBox.TextProperty, myBinding);
        }

        public object SelectedCompany
        {
            get { return GetValue(SelectedCompanyProperty); }
            set { SetValue(SelectedCompanyProperty, value); }
        }

        public static readonly DependencyProperty SelectedCompanyProperty =
            AutoCompleteTextBox.SelectedItemProperty.AddOwner(typeof(TransporterDetails),
                new FrameworkPropertyMetadata(SelectedCompanyPropertyChanged));

        private static void SelectedCompanyPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var cd = obj as TransporterDetails;
            cd.CompanyNameBox.SelectedItem = e.NewValue;
        }
    }
}
