using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Models.Companies;
using Models.DocumentTypes;
using Models.Entities;
using PosizioniRoverfrutta.Annotations;
using PosizioniRoverfrutta.Windows;
using QueryManager;
using QueryManager.Indexes;
using Raven.Client;
using Raven.Client.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PosizioniRoverfrutta.ViewModels
{
    public class TransporterWindowGridViewModel : INotifyPropertyChanged
    {
        public TransporterWindowGridViewModel(IDataStorage dataStorage, IWindowManager windowManager)
        {
            _dataStorage = dataStorage;
            _windowManager = windowManager;
            TransportersList = new ObservableCollection<CustomerRow>();

            LoadAllData();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<CustomerRow> TransportersList { get; private set; }

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
            get { return _selectedTransporter?.CompanyName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    SaveButtonEnabled = false;
                _selectedTransporter.CompanyName = value;
                OnPropertyChanged();
            }
        }

        public string Address
        {
            get { return _selectedTransporter?.Address; }
            set
            {
                _selectedTransporter.Address = value;
                OnPropertyChanged();
            }
        }

        public string City
        {
            get { return _selectedTransporter?.City; }
            set
            {
                _selectedTransporter.City = value;
                OnPropertyChanged();
            }
        }

        public string StateOrProvince
        {
            get { return _selectedTransporter?.StateOrProvince; }
            set
            {
                _selectedTransporter.StateOrProvince = value;
                OnPropertyChanged();
            }
        }

        public string PostCode
        {
            get { return _selectedTransporter?.PostCode; }
            set
            {
                _selectedTransporter.PostCode = value;
                OnPropertyChanged();
            }
        }

        public string Country
        {
            get { return _selectedTransporter?.Country; }
            set
            {
                _selectedTransporter.Country = value;
                OnPropertyChanged();
            }
        }

        public string VatCode
        {
            get { return _selectedTransporter?.VatCode; }
            set
            {
                _selectedTransporter.VatCode = value;
                OnPropertyChanged();
            }
        }

        public string EmailAddress
        {
            get { return _selectedTransporter?.EmailAddress; }
            set
            {
                _selectedTransporter.EmailAddress = value;
                OnPropertyChanged();
            }
        }

        public bool DoNotApplyVat
        {
            get
            {
                return _selectedTransporter != null && _selectedTransporter.DoNotApplyVat;
            }
            set
            {
                _selectedTransporter.DoNotApplyVat = value;
                OnPropertyChanged();
            }
        }

        public bool DeleteButtonEnabled { get; private set; }

        public bool SaveButtonEnabled { get; private set; }

        public bool EditControlsEnabled { get { return _selectedTransporter != null; } }

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
            get { return createCommand ?? (createCommand = new DelegateCommand(CreateNewTransporter)); }
        }

        public ICommand DeleteTransporter
        {
            get { return deleteCommand ?? (deleteCommand = new DelegateCommand(DeleteSelectedTransporter)); }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            if (!propertyName.In("SaveButtonEnabled", "DeleteButtonEnabled", "SelectedTransporter", "SearchBox"))
            {
                if (!SaveButtonEnabled && (!string.IsNullOrWhiteSpace(_selectedTransporter?.CompanyName)))
                {
                    SaveButtonEnabled = true;
                    OnPropertyChanged("SaveButtonEnabled");
                }
            }
        }

        private void LoadAllData()
        {
            TransportersList.Clear();
            using (var session = _dataStorage.CreateSession())
            {
                if (string.IsNullOrWhiteSpace(SearchBox))
                {
                    TransportersList.AddRange(session.Query<CustomerRow, TransportersWithNumberOfDocuments>().Customize(x => x.WaitForNonStaleResultsAsOfNow()).OrderBy(c => c.CompanyName).Skip(skipPositions).Take(100));
                }
                else
                {
                    var transportersQuery = session.Query<CustomerRow, TransportersWithNumberOfDocuments>().Customize(x => x.WaitForNonStaleResultsAsOfNow());
                    var queryByName = SearchBox.Split(' ').Aggregate(transportersQuery, (current, term) => current.Search(c => c.CompanyName, "*" + term + "*", options: SearchOptions.And, escapeQueryOptions: EscapeQueryOptions.AllowAllWildcards));
                    TransportersList.AddRange(queryByName.OrderBy(c => c.CompanyName).Take(100));
                }
            }
            var selectedTransportersId = _selectedTransporter?.Id;
            LoadSelectedTransporter(selectedTransportersId);
        }

        public void LoadSelectedTransporter(string selectedTransporterId)
        {
            if (!string.IsNullOrWhiteSpace(selectedTransporterId))
            {
                using (var session = _dataStorage.CreateSession())
                {
                    _selectedTransporter = session.Load<Transporter>(selectedTransporterId);
                    OnPropertyChanged("EditControlsEnabled");
                }
            }
            else
            {
                _selectedTransporter = null;
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
            SetActionButtonsState(selectedTransporterId);
        }

        private void SetActionButtonsState(string selectedTransporterId)
        {
            DeleteButtonEnabled = _selectedTransporter != null;
            using (var session = _dataStorage.CreateSession())
            {
                if (session.Query<PriceConfirmation>().Count(pc => pc.Transporter.Id.Equals(selectedTransporterId)) > 0
                    || session.Query<LoadingDocument>().Count(ld => ld.Transporter.Id.Equals(selectedTransporterId)) > 0
                    || session.Query<SaleConfirmation>().Count(sc => sc.Transporter.Id.Equals(selectedTransporterId)) > 0)
                    DeleteButtonEnabled = false;
            }
            SaveButtonEnabled = false;
            OnPropertyChanged("DeleteButtonEnabled");
            OnPropertyChanged("SaveButtonEnabled");
        }

        private void SaveAndRefresh()
        {
            string name = CompanyName;
            SaveSelectedTransporter();
            LoadSelectedTransporter(null);
            LoadAllData();
            _windowManager.PopupMessage(string.Format("Trasportatore {0} salvato correttamente", name), "Trasportatore salvato");
        }

        private void SaveSelectedTransporter()
        {
            using (var session = _dataStorage.CreateSession())
            {
                session.Store(_selectedTransporter);
                session.SaveChanges();
            }
        }

        private void CreateNewTransporter()
        {
            _selectedTransporter = new Transporter();
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

        private void DeleteSelectedTransporter()
        {
            string name = _selectedTransporter.CompanyName;
            using (var session = _dataStorage.CreateSession())
            {
                var itemToDelete = session.Load<Transporter>(_selectedTransporter.Id);
                session.Delete<Transporter>(itemToDelete);
                session.SaveChanges();
            }
            LoadAllData();
            _windowManager.PopupMessage(string.Format("Trasportatore {0} cancellato dal database", name), "Trasportatore eliminato");
        }

        private void IncreaseSkip()
        {
            if (TransportersList.Count == 100)
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
        private Transporter _selectedTransporter;
        private ICommand nextPageCommand;
        private ICommand previousPageCommand;
        private ICommand refreshCommand;
        private ICommand saveCommand;
        private ICommand createCommand;
        private ICommand deleteCommand;
    }
}
