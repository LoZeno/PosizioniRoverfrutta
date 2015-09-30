using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Practices.Prism;
using Models.Companies;
using PosizioniRoverfrutta.Annotations;
using PosizioniRoverfrutta.Windows;
using QueryManager;
using QueryManager.QueryHelpers;
using Raven.Client.Linq;

namespace PosizioniRoverfrutta.ViewModels
{
    public class CustomersWindowViewModel : INotifyPropertyChanged
    {
        public CustomersWindowViewModel(IDataStorage dataStorage, IWindowManager windowManager)
        {
            _dataStorage = dataStorage;
            _windowManager = windowManager;
            CustomersList = new ObservableCollection<Customer>();

            LoadCustomersList();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Customer> CustomersList { get; private set; }

        public string SearchBox
        {
            get { return _searchBox; }
            set
            {
                _searchBox = value;
                LoadCustomersList();
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadCustomersList()
        {
            CustomersList.Clear();
            using (var session = _dataStorage.CreateSession())
            {
                if (string.IsNullOrWhiteSpace(SearchBox))
                {
                    CustomersList.AddRange(session.Query<Customer>().OrderBy(c => c.CompanyName).Take(100));
                }
                else
                {
                    var customersQuery = session.FindByPartialName<Customer>(SearchBox);
                    CustomersList.AddRange(customersQuery.OrderBy(c => c.CompanyName).Take(100));
                }
            }
        }

        private readonly IDataStorage _dataStorage;
        private readonly IWindowManager _windowManager;
        private string _searchBox;
    }
}
