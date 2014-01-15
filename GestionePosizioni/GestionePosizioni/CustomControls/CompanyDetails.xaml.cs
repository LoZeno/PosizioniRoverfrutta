using System.Windows.Controls;
using System.Windows.Data;
using dragonz.actb.control;
using dragonz.actb.core;
using dragonz.actb.provider;
using GestionePosizioni.BaseClasses;
using GestionePosizioni.ViewModels;

namespace GestionePosizioni.CustomControls
{
    /// <summary>
    /// Interaction logic for CompanyDetails.xaml
    /// </summary>
    public partial class CompanyDetails : BaseUserControl
    {
        private IAutoCompleteWithReturnValueDataProvider _companyDataProvider;

        public CompanyDetails()
        {
            InitializeComponent();
        }

        public CompanyDetails(ICompanyDetailsViewModel  model, IAutoCompleteWithReturnValueDataProvider companyDataProvider)
            : this()
        {
            _companyDataProvider = companyDataProvider;
            CompanyNameTextBox.AutoCompleteManager.DataProvider = _companyDataProvider;
            CompanyNameTextBox.AutoCompleteManager.Asynchronous = true;

            DataContext = model;
            //var companiesCombo = new Binding("Companies");
            //CompanyNameTextBox.SetBinding(ComboBox.ItemsSourceProperty, companiesCombo);

            //var companyId = new Binding("Id");
            //CompanyNameTextBox.SetBinding(ComboBox.SelectedValueProperty, companyId);
            //CompanyNameTextBox.SetBinding(ComboBox.SelectedValuePathProperty, companyId);
            //var companyName = new Binding("CompanyName");
            //CompanyNameTextBox.SetBinding(ComboBox.TextProperty, companyName);
            //var myBinding = new Binding("Address");
            //myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //myBinding.Mode = BindingMode.TwoWay;
            //Address.SetBinding(TextBox.TextProperty, myBinding);
            //var city = new Binding("City");
            //city.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //City.SetBinding(TextBox.TextProperty, city);
            //var state = new Binding("StateOrProvince");
            //County.SetBinding(TextBox.TextProperty, state);
            //var postcode = new Binding("PostCode");
            //PostalCode.SetBinding(TextBox.TextProperty, postcode);
            //var country = new Binding("Country");
            //Country.SetBinding(ComboBox.TextProperty, country);
            //var vat = new Binding("VatCode");
            //VatCode.SetBinding(TextBox.TextProperty, vat);
        }
    }
}
