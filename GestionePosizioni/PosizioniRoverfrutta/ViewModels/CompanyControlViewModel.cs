using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Models.Companies;
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
            get => _company;
            set
            {
                _company = value;
                OnPropertyChanged(string.Empty);
            }
        }

        public string Id
        {
            get => Company.Id;
            set
            {
                Company.Id = value;
                OnPropertyChanged();
            }
        }

        public string CompanyName
        {
            get => Company.CompanyName;
            set => Company = LoadCustomerByName(value);
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
            get => Company.Address;
            set
            {
                Company.Address = value; 
                OnPropertyChanged();
            }
        }

        public string City
        {
            get => Company.City;
            set
            {
                Company.City = value;
                OnPropertyChanged();
            }
        }

        public string StateOrProvince
        {
            get => Company.StateOrProvince;
            set
            {
                Company.StateOrProvince = value;
                OnPropertyChanged();
            }
        }

        public string PostCode
        {
            get => Company.PostCode;
            set
            {
                Company.PostCode = value;
                OnPropertyChanged();
            }
        }

        public string Country
        {
            get => Company.Country;
            set
            {
                Company.Country = value;
                OnPropertyChanged();
            }
        }

        public string VatCode
        {
            get => Company.VatCode;
            set
            {
                Company.VatCode = value;
                OnPropertyChanged();
            }
        }

        public string EmailAddress
        {
            get => Company.EmailAddress;
            set
            {
                Company.EmailAddress = value;
                OnPropertyChanged();
            }
        }

        public bool DoNotApplyVat
        {
            get => Company.DoNotApplyVat;
            set
            {
                Company.DoNotApplyVat = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private T _company;
        private readonly IDataStorage _dataStorage;
        private readonly string _objectName;
    }
}
