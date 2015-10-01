using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Models.Companies;
using PosizioniRoverfrutta.Annotations;
using PosizioniRoverfrutta.Windows;
using QueryManager;
using QueryManager.QueryHelpers;
using Raven.Client.Linq;

namespace PosizioniRoverfrutta.ViewModels
{
    public class CustomersWindowGridViewModel : INotifyPropertyChanged
    {
        public CustomersWindowGridViewModel(IDataStorage dataStorage, IWindowManager windowManager)
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

        public ICommand NextPage
        {
            get { return nextPageCommand ?? (nextPageCommand = new DelegateCommand(IncreaseSkip)); } 
        }

        public ICommand PreviousPage 
        { 
            get { return previousPageCommand ?? (previousPageCommand = new DelegateCommand(DecreaseSkip)); }
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
                    CustomersList.AddRange(session.Query<Customer>().OrderBy(c => c.CompanyName).Skip(skipPositions).Take(100));
                }
                else
                {
                    var customersQuery = session.FindByPartialName<Customer>(SearchBox);
                    CustomersList.AddRange(customersQuery.OrderBy(c => c.CompanyName).Take(100));
                }
            }
        }

        private void IncreaseSkip()
        {
            if (CustomersList.Count == 100)
            {
                skipPositions += 100;
                LoadCustomersList();
            }
        }

        private void DecreaseSkip()
        {
            if (skipPositions != 0)
            {
                skipPositions -= 100;
                if (skipPositions < 0)
                {
                    skipPositions = 0;
                }
                LoadCustomersList();
            }
        }

        private readonly IDataStorage _dataStorage;
        private readonly IWindowManager _windowManager;
        private string _searchBox;
        private ICommand nextPageCommand;
        private int skipPositions = 0;
        private ICommand previousPageCommand;
    }
}
