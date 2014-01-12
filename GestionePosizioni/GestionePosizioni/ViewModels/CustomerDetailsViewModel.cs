using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Models;
using QueryManager;
using QueryManager.Repositories;

namespace GestionePosizioni.ViewModels
{
    public class CustomerDetailsViewModel : ICompanyDetailsViewModel
    {
        private CompanyBase _customer;
        private ICustomerRepository _queryManager;
        

        public CustomerDetailsViewModel(Customer customer, ICustomerRepository queryManager)
        {
            if (customer == null)
            {
                customer = new Customer();
            }
            _customer = customer;
            _queryManager = queryManager;
        }

        public CustomerDetailsViewModel(ICustomerRepository queryManager)
            :this (null, queryManager)
        {
            
        }

        public CompanyBase Company
        {
            get { return _customer; }
            set
            {
                _customer = value ?? new Customer();
                OnPropertyChanged("Company");
            }
        }

        public string Id
        {
            get { return _customer.Id; }
            set
            {
                Company = _queryManager.FindById(value);
                OnPropertyChanged("Id");
            }
        }

        public string CompanyName
        {
            get { return _customer.CompanyName; }
            set
            {
                _customer.CompanyName = value;
                Companies = new ObservableCollection<CompanyBase>(_queryManager.FindByPartialName(value));
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

        private ICommand saveCommand;

        public ICommand Save
        {
            get
            {
                if (saveCommand == null)
                {
                    saveCommand = new DelegateCommand(delegate()
                    {
                        if (string.IsNullOrWhiteSpace(Id))
                        {
                            _queryManager.Add((Customer)_customer);
                        }
                        _queryManager.Save();
                    });
                }
                return saveCommand;
            }
        }
    }
}
