using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using dragonz.actb.control;
using dragonz.actb.provider;
using GestionePosizioni.BaseClasses;
using GestionePosizioni.ViewModels;
using Models;

namespace GestionePosizioni.CustomControls
{
    /// <summary>
    /// Interaction logic for CompanyDetails.xaml
    /// </summary>
    public partial class CompanyDetails : BaseUserControl
    {
        private IAutoCompleteWithReturnValueDataProvider _companyDataProvider;
        private ICompanyDetailsViewModel _model;

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
            _model = model;
            DataContext = _model;

            var companyBinding = new Binding
            {
                Source = _model,
                Path = new PropertyPath("Company"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            CompanyNameTextBox.SetBinding(AutoCompleteTextBox.SelectedItemProperty, companyBinding);
            SetBinding(CompanyDetails.SelectedCompanyProperty, companyBinding);

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
            get { return (object)this.GetValue(SelectedCompanyProperty); }
            set { SetValue(SelectedCompanyProperty, value); }
        }

        public static readonly DependencyProperty SelectedCompanyProperty = DependencyProperty.Register(
    "SelectedCompany", typeof(object), typeof(CompanyDetails), new PropertyMetadata(false));
    }
}
