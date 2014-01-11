using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace GestionePosizioni.ViewModels
{
    public class CustomerDetailsViewModel : ICompanyDetailsViewModel
    {
        private Customer _customer;

        public CustomerDetailsViewModel(Customer customer)
        {
            if (customer == null)
            {
                customer = new Customer();
            }
            _customer = customer;
        }

        public string Id
        {
            get { return _customer.Id; }
            set
            {
                _customer.Id = value;
                OnPropertyChanged("Id");
            }
        }

        public string CompanyName
        {
            get { return _customer.CompanyName; }
            set
            {
                _customer.CompanyName = value;
                OnPropertyChanged("CompanyName");
            }
        }

        public string Address
        {
            get { return _customer.Address; }
            set
            {
                _customer.Address = value;
                OnPropertyChanged("Address");
            }
        }

        public string City
        {
            get { return _customer.City; }
            set
            {
                _customer.City = value;
                OnPropertyChanged("City");
            }
        }

        public string StateOrProvince
        {
            get { return _customer.StateOrProvince; }
            set
            {
                _customer.StateOrProvince = value;
                OnPropertyChanged("StateOrProvince");
            }
        }

        public string PostCode
        {
            get { return _customer.PostCode; }
            set
            {
                _customer.PostCode = value;
                OnPropertyChanged("PostCode");
            }
        }

        public string Country
        {
            get { return _customer.Country; }
            set
            {
                _customer.Country = value;
                OnPropertyChanged("Country");
            }
        }

        public string VatCode
        {
            get { return _customer.VatCode; }
            set
            {
                _customer.VatCode = value;
                OnPropertyChanged("VatCode");
            }
        }

        public ObservableCollection<CompanyBase> Companies { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
