using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using dragonz.actb.control;
using Models.Companies;
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

        public CompanyDetails(IDataStorage dataStorage, CompanyControlViewModel<Customer> viewModel)
            : this()
        {
            var companyDataProvider = new CustomerNamesAutoCompleteBoxProvider<Customer>(dataStorage);
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
            SetCheckboxBinding(DoNotApplyVatCheckBox, "DoNotApplyVat");
        }

        private void SetCheckboxBinding(CheckBox checkBoxControl, string propertyName)
        {
            var myBinding = new Binding(propertyName)
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            checkBoxControl.SetBinding(ToggleButton.IsCheckedProperty, myBinding);
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
            cd.CompanyNameBox.SelectedItem = e.NewValue;
        }
    }
}
