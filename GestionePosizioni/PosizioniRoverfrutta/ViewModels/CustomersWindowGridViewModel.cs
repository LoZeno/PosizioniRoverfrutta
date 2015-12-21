using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Models.Companies;
using Models.DocumentTypes;
using Models.Entities;
using PosizioniRoverfrutta.Annotations;
using PosizioniRoverfrutta.Windows;
using QueryManager;
using QueryManager.Indexes;
using QueryManager.QueryHelpers;
using Raven.Client;
using Raven.Client.Linq;

namespace PosizioniRoverfrutta.ViewModels
{
    public class CustomersWindowGridViewModel : INotifyPropertyChanged
    {
        public CustomersWindowGridViewModel(IDataStorage dataStorage, IWindowManager windowManager)
        {
            _dataStorage = dataStorage;
            _windowManager = windowManager;
            CustomersList = new ObservableCollection<CustomerRow>();

            LoadAllData();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<CustomerRow> CustomersList { get; private set; }

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

        public string CompanyName
        {
            get { return _selectedCustomer?.CompanyName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    SaveButtonEnabled = false;
                _selectedCustomer.CompanyName = value;
                OnPropertyChanged();
            }
        }

        public string Address
        {
            get { return _selectedCustomer?.Address; }
            set
            {
                _selectedCustomer.Address = value;
                OnPropertyChanged();
            }
        }

        public string City
        {
            get { return _selectedCustomer?.City; }
            set
            {
                _selectedCustomer.City = value;
                OnPropertyChanged();
            }
        }

        public string StateOrProvince
        {
            get { return _selectedCustomer?.StateOrProvince; }
            set
            {
                _selectedCustomer.StateOrProvince = value;
                OnPropertyChanged();
            }
        }

        public string PostCode
        {
            get { return _selectedCustomer?.PostCode; }
            set
            {
                _selectedCustomer.PostCode = value;
                OnPropertyChanged();
            }
        }

        public string Country
        {
            get { return _selectedCustomer?.Country; }
            set
            {
                _selectedCustomer.Country = value;
                OnPropertyChanged();
            }
        }

        public string VatCode
        {
            get { return _selectedCustomer?.VatCode; }
            set
            {
                _selectedCustomer.VatCode = value;
                OnPropertyChanged();
            }
        }

        public string EmailAddress
        {
            get { return _selectedCustomer?.EmailAddress; }
            set
            {
                _selectedCustomer.EmailAddress = value;
                OnPropertyChanged();
            }
        }

        public bool DoNotApplyVat
        {
            get
            {
                return _selectedCustomer != null && _selectedCustomer.DoNotApplyVat;
            }
            set
            {
                _selectedCustomer.DoNotApplyVat = value;
                OnPropertyChanged();
            }
        }

        public bool DeleteButtonEnabled { get; private set; }

        public bool SaveButtonEnabled { get; private set; }

        public bool EditControlsEnabled { get { return _selectedCustomer != null; } }

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

        public ICommand CreateNew
        {
            get { return createCommand ?? (createCommand = new DelegateCommand(CreateNewCustomer)); }
        }

        public ICommand DeleteCustomer
        {
            get { return deleteCommand ?? (deleteCommand = new DelegateCommand(DeleteSelectedCustomer)); }
        }

        public void LoadSelectedCustomer(string selectedCustomerId)
        {
            if (!string.IsNullOrWhiteSpace(selectedCustomerId))
            {
                using (var session = _dataStorage.CreateSession())
                {
                    _selectedCustomer = session.Load<Customer>(selectedCustomerId);
                    OnPropertyChanged("EditControlsEnabled");
                }
            }
            else
            {
                _selectedCustomer = null;
                OnPropertyChanged("EditControlsEnabled");
            }

            OnPropertyChanged("CompanyName");
            OnPropertyChanged("Address");
            OnPropertyChanged("City");
            OnPropertyChanged("StateOrProvince");
            OnPropertyChanged("PostCode");
            OnPropertyChanged("Country");
            OnPropertyChanged("VatCode");
            OnPropertyChanged("EmailAddress");
            OnPropertyChanged("DoNotApplyVat");
            SetActionButtonsState(selectedCustomerId);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (!propertyName.In("SaveButtonEnabled", "DeleteButtonEnabled", "SelectedCustomer", "SearchBox"))
            {
                if (!SaveButtonEnabled && (!string.IsNullOrWhiteSpace(_selectedCustomer?.CompanyName)))
                {
                    SaveButtonEnabled = true;
                    OnPropertyChanged("SaveButtonEnabled");
                }
            }
        }

        private void LoadAllData()
        {
            CustomersList.Clear();
            using (var session = _dataStorage.CreateSession())
            {
                if (string.IsNullOrWhiteSpace(SearchBox))
                {
                    CustomersList.AddRange(session.Query<CustomerRow, CustomersWithNumberOfDocuments>().OrderBy(c => c.CompanyName).Skip(skipPositions).Take(100).ToList());
                }
                else
                {
                    var customersQuery = session.Query<CustomerRow, CustomersWithNumberOfDocuments>();
                    var queryByName = SearchBox.Split(' ').Aggregate(customersQuery, (current, term) => current.Search(c => c.CompanyName, "*" + term + "*", options: SearchOptions.And, escapeQueryOptions: EscapeQueryOptions.AllowAllWildcards));
                    CustomersList.AddRange(queryByName.OrderBy(c => c.CompanyName).Take(100).ToList());
                }
            }
            var selectedCustomerId = _selectedCustomer?.Id;
            LoadSelectedCustomer(selectedCustomerId);
        }

        private void SetActionButtonsState(string selectedCustomerId)
        {
            DeleteButtonEnabled = _selectedCustomer != null;
            using (var session = _dataStorage.CreateSession())
            {
                if (session.Query<PriceConfirmation>().Count(pc => pc.Customer.Id.Equals(selectedCustomerId) || pc.Provider.Id.Equals(selectedCustomerId)) > 0
                    || session.Query<LoadingDocument>().Count(ld => ld.Customer.Id.Equals(selectedCustomerId) || ld.Provider.Id.Equals(selectedCustomerId)) > 0
                    || session.Query<SaleConfirmation>().Count(sc => sc.Customer.Id.Equals(selectedCustomerId) || sc.Provider.Id.Equals(selectedCustomerId)) > 0)
                    DeleteButtonEnabled = false;
            }
            SaveButtonEnabled = false;
            OnPropertyChanged("DeleteButtonEnabled");
            OnPropertyChanged("SaveButtonEnabled");
        }

        private void SaveAndRefresh()
        {
            string name = CompanyName;
            SaveSelectedCustomer();
            LoadSelectedCustomer(null);
            LoadAllData();
            _windowManager.PopupMessage(string.Format("Cliente {0} salvato correttamente", name), "Cliente salvato");
        }

        private void SaveSelectedCustomer()
        {
            using (var session = _dataStorage.CreateSession())
            {
                session.Store(_selectedCustomer);
                session.SaveChanges();
            }
        }

        private void CreateNewCustomer()
        {
            _selectedCustomer = new Customer();
            OnPropertyChanged("EditControlsEnabled");
            OnPropertyChanged("CompanyName");
            OnPropertyChanged("Address");
            OnPropertyChanged("City");
            OnPropertyChanged("StateOrProvince");
            OnPropertyChanged("PostCode");
            OnPropertyChanged("Country");
            OnPropertyChanged("VatCode");
            OnPropertyChanged("EmailAddress");
            OnPropertyChanged("DoNotApplyVat");
            DeleteButtonEnabled = false;
            OnPropertyChanged("DeleteButtonEnabled");
        }

        private void DeleteSelectedCustomer()
        {
            string name = _selectedCustomer.CompanyName;
            using (var session = _dataStorage.CreateSession())
            {
                var itemToDelete = session.Load<Customer>(_selectedCustomer.Id);
                session.Delete<Customer>(itemToDelete);
                session.SaveChanges();
            }
            LoadAllData();
            _windowManager.PopupMessage(string.Format("Cliente {0} cancellato dal database", name), "Cliente eliminato");
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
        private ICommand createCommand;
        private ICommand deleteCommand;
    }
}
