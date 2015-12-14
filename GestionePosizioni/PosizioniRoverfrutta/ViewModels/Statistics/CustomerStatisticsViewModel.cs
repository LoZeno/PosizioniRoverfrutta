using Models.Companies;
using PosizioniRoverfrutta.Annotations;
using QueryManager;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PosizioniRoverfrutta.ViewModels.Statistics
{
    public class CustomerStatisticsViewModel : INotifyPropertyChanged
    {
        public CustomerStatisticsViewModel(IDataStorage dataStorage, string customerId)
        {
            _dataStorage = dataStorage;

            using (var session = _dataStorage.CreateSession())
            {
                _customer = session.Load<Customer>(customerId);
            }
        }

        public string CustomerName
        {
            get { return _customer.CompanyName; }
        }

        public string Address
        {
            get { return _customer.Address; }
        }
        public string City
        {
            get { return _customer.City; }
        }
        public string Country
        {
            get { return _customer.Country; }
        }
        public string EmailAddress
        {
            get { return _customer.EmailAddress; }
        }
        public string PostCode
        {
            get { return _customer.PostCode; }
        }
        public string StateOrProvince
        {
            get { return _customer.StateOrProvince; }
        }
        public string VatCode
        {
            get { return _customer.VatCode; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private Customer _customer;
        private IDataStorage _dataStorage;
    }
}
