using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Models;
using PosizioniRoverfrutta.Annotations;
using QueryManager;

namespace PosizioniRoverfrutta.ViewModels
{
    public class CompanyControlViewModel<T> : INotifyPropertyChanged where T : CompanyBase, new()
    {
        public CompanyControlViewModel(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
            _company = new T();
            _objectName = typeof(T).Name;
        }

        public T Company
        {
            get { return _company; }
            set
            {
                _company = value;
                OnPropertyChanged();
                OnPropertyChanged("Id");
                OnPropertyChanged("CompanyName");
                OnPropertyChanged("Address");
                OnPropertyChanged("PostCode");
                OnPropertyChanged("City");
                OnPropertyChanged("StateOrProvince");
                OnPropertyChanged("Country");
                OnPropertyChanged("VatCode");
            }
        }

        public string Id
        {
            get { return Company.Id; }
            set
            {
                Company.Id = value;
                OnPropertyChanged();
            }
        }

        public string CompanyName
        {
            get { return Company.CompanyName; }
            set {
                Company = LoadCustomerByName(value);
            }
        }

        private T LoadCustomerByName(string companyName)
        {
            using (var session = _dataStorage.CreateSession())
            {
                var customer = session.Query<T>(_objectName + "/ByCompanyName").FirstOrDefault(c => c.CompanyName.Equals(companyName));
                return customer ?? new T
                {
                    CompanyName = companyName
                };
            }
        }

        public string Address
        {
            get { return Company.Address; }
            set
            {
                Company.Address = value; 
                OnPropertyChanged();
            }
        }

        public string City
        {
            get { return Company.City; }
            set
            {
                Company.City = value;
                OnPropertyChanged();
            }
        }

        public string StateOrProvince
        {
            get { return Company.StateOrProvince; }
            set
            {
                Company.StateOrProvince = value;
                OnPropertyChanged();
            }
        }

        public string PostCode
        {
            get { return Company.PostCode; }
            set
            {
                Company.PostCode = value;
                OnPropertyChanged();
            }
        }

        public string Country
        {
            get { return Company.Country; }
            set
            {
                Company.Country = value;
                OnPropertyChanged();
            }
        }

        public string VatCode
        {
            get { return Company.VatCode; }
            set
            {
                Company.VatCode = value;
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

        private T _company;
        private readonly IDataStorage _dataStorage;
        private readonly string _objectName;
    }
}
