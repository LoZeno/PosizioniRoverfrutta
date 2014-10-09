using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Models;
using PosizioniRoverfrutta.Annotations;
using QueryManager;

namespace PosizioniRoverfrutta.ViewModels
{
    public class CustomerControlViewModel : INotifyPropertyChanged
    {
        public CustomerControlViewModel(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
            _customer = new Customer();
        }

        public Customer Customer
        {
            get { return _customer; }
            set
            {
                _customer = value;
                OnPropertyChanged();
                OnPropertyChanged("Id");
                OnPropertyChanged("CompanyName");
                OnPropertyChanged("Address");
                OnPropertyChanged("City");
                OnPropertyChanged("StateOrProvince");
                OnPropertyChanged("Country");
                OnPropertyChanged("VatCode");
            }
        }

        public string Id
        {
            get { return Customer.Id; }
            set
            {
                Customer.Id = value;
                OnPropertyChanged();
            }
        }

        public string CompanyName
        {
            get { return Customer.CompanyName; }
            set {
                LoadCustomerByName(value);
            }
        }

        private void LoadCustomerByName(string companyName)
        {
            Customer customer;
            using (var session = _dataStorage.CreateSession())
            {
                customer = session.Query<Customer>("Customer/ByCompanyName").FirstOrDefault(c => c.CompanyName.Equals(companyName));
            }
            if (customer != null)
            {
                Customer = customer;
            }
            else
            {
                Customer = new Customer
                {
                    CompanyName = companyName
                };
            }
        }

        public string Address
        {
            get { return Customer.Address; }
            set
            {
                Customer.Address = value; 
                OnPropertyChanged();
            }
        }

        public string City
        {
            get { return Customer.City; }
            set
            {
                Customer.City = value;
                OnPropertyChanged();
            }
        }

        public string StateOrProvince
        {
            get { return Customer.StateOrProvince; }
            set
            {
                Customer.StateOrProvince = value;
                OnPropertyChanged();
            }
        }

        public string PostCode
        {
            get { return Customer.PostCode; }
            set
            {
                Customer.PostCode = value;
                OnPropertyChanged();
            }
        }

        public string Country
        {
            get { return Customer.Country; }
            set
            {
                Customer.Country = value;
                OnPropertyChanged();
            }
        }

        public string VatCode
        {
            get { return Customer.VatCode; }
            set
            {
                Customer.VatCode = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private Customer _customer;
        private readonly IDataStorage _dataStorage;
    }
}
