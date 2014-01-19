using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using dragonz.actb.control;
using dragonz.actb.core;
using dragonz.actb.provider;
using GestionePosizioni.BaseClasses;
using GestionePosizioni.ViewModels;
using iTextSharp.text.pdf.qrcode;

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

            var companyBinding = new Binding();
            companyBinding.Source = model;
            companyBinding.Path = new PropertyPath("Company");
            companyBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            companyBinding.Mode = BindingMode.TwoWay;
            CompanyNameTextBox.SetBinding(AutoCompleteTextBox.SelectedItemProperty, companyBinding);

            var myBinding = new Binding("Address");
            myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            myBinding.Mode = BindingMode.TwoWay;
            Address.SetBinding(TextBox.TextProperty, myBinding);
            var city = new Binding("City");
            city.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            city.Mode = BindingMode.TwoWay;
            City.SetBinding(TextBox.TextProperty, city);
            var state = new Binding("StateOrProvince");
            state.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            state.Mode = BindingMode.TwoWay;
            County.SetBinding(TextBox.TextProperty, state);
            var postcode = new Binding("PostCode");
            postcode.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            postcode.Mode = BindingMode.TwoWay;
            PostalCode.SetBinding(TextBox.TextProperty, postcode);
            var country = new Binding("Country");
            country.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            country.Mode = BindingMode.TwoWay;
            Country.SetBinding(ComboBox.TextProperty, country);
            var vat = new Binding("VatCode");
            vat.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            vat.Mode = BindingMode.TwoWay;
            VatCode.SetBinding(TextBox.TextProperty, vat);
        }
    }
}
