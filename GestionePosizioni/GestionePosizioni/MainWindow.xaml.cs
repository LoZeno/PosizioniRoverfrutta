using System.Windows;
using dragonz.actb.provider;
using GestionePosizioni.BaseClasses;
using GestionePosizioni.CustomControls;
using GestionePosizioni.CustomControls.ControlServices;
using GestionePosizioni.ViewModels;
using QueryManager.Repositories;

namespace GestionePosizioni
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : BaseWindow
    {
        private CustomerDetailsViewModel cvm;

        public MainWindow()
        {
            InitializeComponent();
            var customerRepo = new CustomerRepository(this.DatabaseSession);
            var dataProviderForCustomer = new CustomerAutoCompleteBoxProvider(customerRepo);
            cvm = new CustomerDetailsViewModel(customerRepo);
            CompanyDetails cd = new CompanyDetails(cvm, dataProviderForCustomer);
            MainPanel.Children.Add(cd);
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            cvm.Save.Execute(null);
        }
    }
}
