using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using dragonz.actb.control;
using Models;
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
            var companyDataProvider = new CustomerAutoCompleteBoxProvider<Customer>(dataStorage);
            CompanyNameTextBox.AutoCompleteManager.DataProvider = companyDataProvider;
            CompanyNameTextBox.AutoCompleteManager.Asynchronous = true;
            DataContext = viewModel;

            var companyBinding = new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("Customer"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            CompanyNameTextBox.SetBinding(AutoCompleteTextBox.SelectedItemProperty, companyBinding);
            SetBinding(SelectedCompanyProperty, companyBinding);

            SetTextboxBinding(Address, "Address");
            SetTextboxBinding(City, "City");
            SetTextboxBinding(County, "StateOrProvince");
            SetTextboxBinding(PostalCode, "PostCode");
            SetTextboxBinding(Country, "Country");
            SetTextboxBinding(VatCode, "VatCode");
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
            AutoCompleteTextBox.SelectedItemProperty.AddOwner(typeof(CompanyDetails),
                new FrameworkPropertyMetadata(SelectedCompanyPropertyChanged));

        private static void SelectedCompanyPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var cd = obj as CompanyDetails;
            cd.CompanyNameTextBox.SelectedItem = e.NewValue;
        }
    }
}
