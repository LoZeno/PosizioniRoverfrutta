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

            LoadAllData();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Customer> CustomersList { get; private set; }

        public string SearchBox
        {
            get { return _searchBox; }
            set
            {
                _searchBox = value;
                LoadAllData();
                OnPropertyChanged();
            }
        }

        public Customer SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged();
                OnPropertyChanged("CompanyName");
                OnPropertyChanged("Address");
                OnPropertyChanged("City");
                OnPropertyChanged("StateOrProvince");
                OnPropertyChanged("PostCode");
                OnPropertyChanged("Country");
                OnPropertyChanged("VatCode");
                OnPropertyChanged("EmailAddress");
                OnPropertyChanged("DoNotApplyVat");
                SetActionButtonsState();
            }
        }

        public string CompanyName
        {
            get { return _selectedCustomer.CompanyName; }
            set
            {
                _selectedCustomer.CompanyName = value;
                OnPropertyChanged();
            }
        }

        public string Address
        {
            get { return _selectedCustomer.Address; }
            set
            {
                _selectedCustomer.Address = value;
                OnPropertyChanged();
            }
        }

        public string City
        {
            get { return _selectedCustomer.City; }
            set
            {
                _selectedCustomer.City = value;
                OnPropertyChanged();
            }
        }

        public string StateOrProvince
        {
            get { return _selectedCustomer.StateOrProvince; }
            set
            {
                _selectedCustomer.StateOrProvince = value;
                OnPropertyChanged();
            }
        }

        public string PostCode
        {
            get { return _selectedCustomer.PostCode; }
            set
            {
                _selectedCustomer.PostCode = value;
                OnPropertyChanged();
            }
        }

        public string Country
        {
            get { return _selectedCustomer.Country; }
            set
            {
                _selectedCustomer.Country = value;
                OnPropertyChanged();
            }
        }

        public string VatCode
        {
            get { return _selectedCustomer.VatCode; }
            set
            {
                _selectedCustomer.VatCode = value;
                OnPropertyChanged();
            }
        }

        public string EmailAddress
        {
            get { return _selectedCustomer.EmailAddress; }
            set
            {
                _selectedCustomer.EmailAddress = value;
                OnPropertyChanged();
            }
        }

        public bool DoNotApplyVat
        {
            get { return _selectedCustomer.DoNotApplyVat; }
            set
            {
                _selectedCustomer.DoNotApplyVat = value;
                OnPropertyChanged();
            }
        }

        public bool DeleteButtonEnabled
        {
            get { return _deleteButtonEnabled; }
            set
            {
                _deleteButtonEnabled = value;
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

        public ICommand Refresh
        {
            get { return refreshCommand ?? (refreshCommand = new DelegateCommand(LoadAllData)); }
        }

        public ICommand Save
        {
            get { return saveCommand ?? (saveCommand = new DelegateCommand(SaveAndRefresh)); }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadAllData()
        {
            CustomersList.Clear();
            string selectedCustomerId = _selectedCustomer != null ? _selectedCustomer.Id: string.Empty;
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
                if (!string.IsNullOrWhiteSpace(selectedCustomerId))
                {
                    SelectedCustomer = session.Load<Customer>(selectedCustomerId);
                }
            }
        }

        private void SaveAndRefresh()
        {
            SaveSelectedCustomer();
            LoadAllData();
            SetActionButtonsState();
        }

        private void SetActionButtonsState()
        {
            _deleteButtonEnabled = _selectedCustomer != null;
        }

        private void SaveSelectedCustomer()
        {
            using (var session = _dataStorage.CreateSession())
            {
                session.Store(_selectedCustomer);
                session.SaveChanges();
            }
        }

        private void IncreaseSkip()
        {
            if (CustomersList.Count == 100)
            {
                skipPositions += 100;
                LoadAllData();
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
                LoadAllData();
            }
        }

        private readonly IDataStorage _dataStorage;
        private readonly IWindowManager _windowManager;
        private string _searchBox;
        private int skipPositions = 0;
        private Customer _selectedCustomer;
        private ICommand nextPageCommand;
        private ICommand previousPageCommand;
        private ICommand refreshCommand;
        private ICommand saveCommand;
        private bool _deleteButtonEnabled;
    }
}
