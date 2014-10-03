using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using dragonz.actb.control;
using PosizioniRoverfrutta.Services;
using PosizioniRoverfrutta.ViewModels;
using QueryManager;

namespace PosizioniRoverfrutta.CustomControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class CompanyDetails : UserControl
    {
        public CompanyDetails()
        {
            InitializeComponent();
        }

        public CompanyDetails(IDataStorage dataStorage, CustomerControlViewModel viewModel)
            : this()
        {
            var companyDataProvider = new CustomerAutoCompleteBoxProvider(dataStorage);
            CompanyNameTextBox.AutoCompleteManager.DataProvider = companyDataProvider;
            CompanyNameTextBox.AutoCompleteManager.Asynchronous = true;
            DataContext = viewModel;

            var companyBinding = new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("Company"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            CompanyNameTextBox.SetBinding(AutoCompleteTextBox.SelectedItemProperty, companyBinding);
            SetBinding(SelectedCompanyProperty, companyBinding);

            var myBinding = new Binding("Address")
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            Address.SetBinding(TextBox.TextProperty, myBinding);
            var city = new Binding("City")
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            City.SetBinding(TextBox.TextProperty, city);
            var state = new Binding("StateOrProvince")
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            County.SetBinding(TextBox.TextProperty, state);
            var postcode = new Binding("PostCode")
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            PostalCode.SetBinding(TextBox.TextProperty, postcode);
            var country = new Binding("Country")
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            Country.SetBinding(ComboBox.TextProperty, country);
            var vat = new Binding("VatCode")
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            VatCode.SetBinding(TextBox.TextProperty, vat);
        }

        public object SelectedCompany
        {
            get { return GetValue(SelectedCompanyProperty); }
            set { SetValue(SelectedCompanyProperty, value); }
        }

        public static readonly DependencyProperty SelectedCompanyProperty =
            AutoCompleteTextBox.SelectedItemProperty.AddOwner(typeof(CompanyDetails),
                new FrameworkPropertyMetadata(SelectedCompanyPropertyChanged));

        private static void SelectedCompanyPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var cd = obj as CompanyDetails;
            cd.CompanyNameTextBox.SelectedItem = e.NewValue;
        }
    }
}
