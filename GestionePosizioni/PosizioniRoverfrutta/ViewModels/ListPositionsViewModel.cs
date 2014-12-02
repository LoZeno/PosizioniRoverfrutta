using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Models.Entities;
using PosizioniRoverfrutta.Annotations;
using QueryManager;
using QueryManager.Indexes;
using Raven.Abstractions.Extensions;
using Raven.Client.Linq;

namespace PosizioniRoverfrutta.ViewModels
{
    public class ListPositionsViewModel : INotifyPropertyChanged
    {
        public ListPositionsViewModel(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
            PositionsList = new ObservableCollection<PositionsListRow>();
            RefreshData();
        }

        public bool HasFocus
        {
            get { return _hasFocus; }
            set
            {
                _hasFocus = value;
                RefreshData();
                OnPropertyChanged();
            }
        }

        public string CompanyName
        {
            get { return _companyName; }
            set
            {
                _companyName = value;
                RefreshData();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PositionsListRow> PositionsList { get; set; }

        public PositionsListRow SelectedPosition
        {
            get
            {
                return _selectedRow;
            }
            set
            {
                _selectedRow = value;
                OnPropertyChanged();
                OnPropertyChanged("OpenSaleConfirmationIsEnabled");
                OnPropertyChanged("OpenLoadingDocumentIsEnabled");
                OnPropertyChanged("OpenPriceConfirmationIsEnabled");
            }
        }

        public DateTime? FromDate
        {
            get
            {
                return _fromDate;
            }
            set
            {
                _fromDate = value;
                RefreshData();
                OnPropertyChanged();
            }
        }

        public DateTime? ToDate
        {
            get
            {
                return _toDate;
            }
            set
            {
                _toDate = value;
                RefreshData();
                OnPropertyChanged();
            }
        }

        public bool OpenSaleConfirmationIsEnabled
        {
            get
            {
                return _selectedRow != null;
            }
        }

        public bool OpenLoadingDocumentIsEnabled
        {
            get { return _selectedRow != null && _selectedRow.HasLoadingDocument; }
        }

        public bool OpenPriceConfirmationIsEnabled
        {
            get { return _selectedRow != null && _selectedRow.HasPriceConfirmation; }
        }

        public ICommand Refresh
        {
            get { return refreshCommand ?? (refreshCommand = new DelegateCommand(RefreshData)); }
        }

        public ICommand NextPage
        {
            get { return nextPageCommand ?? (nextPageCommand = new DelegateCommand(IncreaseSkip)); }
        }

        public ICommand PreviousPage
        {
            get { return previousPageCommand ?? (previousPageCommand = new DelegateCommand(DecreaseSkip)); }
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
                RefreshData();
            }
        }

        private void IncreaseSkip()
        {
            if (PositionsList.Count == 100)
            {
                skipPositions += 100;
                RefreshData();
            }
        }

        private void RefreshData()
        {
            PositionsList.Clear();
            using (var session = _dataStorage.CreateSession())
            {
                var baseQuery = session.Query<PositionsListRow, AllPositions>();
                
                baseQuery = ApplyBeforeDateFilter(baseQuery);
                
                baseQuery = ApplyAfterDateFilter(baseQuery);

                baseQuery = ApplyCustomerNameFilter(baseQuery);

                var results = baseQuery.OrderByDescending(lop => lop.ProgressiveNumber).Skip(skipPositions).Take(100).ToList();
                PositionsList.AddRange(results);
            }
        }

        private IRavenQueryable<PositionsListRow> ApplyCustomerNameFilter(IRavenQueryable<PositionsListRow> baseQuery)
        {
            if (!string.IsNullOrWhiteSpace(_companyName))
            {
                baseQuery = baseQuery.Where(sc => sc.CustomerName == _companyName || sc.ProviderName == _companyName);
            }
            return baseQuery;
        }

        private IRavenQueryable<PositionsListRow> ApplyAfterDateFilter(IRavenQueryable<PositionsListRow> baseQuery)
        {
            if (_fromDate.HasValue)
            {
                baseQuery = baseQuery.Where(sc => sc.ShippingDate >= _fromDate);
            }
            return baseQuery;
        }

        private IRavenQueryable<PositionsListRow> ApplyBeforeDateFilter(IRavenQueryable<PositionsListRow> baseQuery)
        {
            if (_toDate.HasValue)
            {
                baseQuery = baseQuery.Where(sc => sc.ShippingDate <= _toDate);
            }
            return baseQuery;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _hasFocus;
        private readonly IDataStorage _dataStorage;
        private string _companyName;
        private DateTime? _fromDate;
        private DateTime? _toDate;
        private PositionsListRow _selectedRow;
        private ICommand refreshCommand;
        private int skipPositions = 0;
        private ICommand nextPageCommand;
        private ICommand previousPageCommand;
    }
}