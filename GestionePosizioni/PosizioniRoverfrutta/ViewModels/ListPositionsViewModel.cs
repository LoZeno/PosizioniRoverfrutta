using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Models.Companies;
using Models.DocumentTypes;
using Models.Entities;
using PosizioniRoverfrutta.Annotations;
using QueryManager;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Raven.Client.Linq;
using Raven.Database.Linq.PrivateExtensions;

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
                LoadCompanyId();
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

        private void LoadCompanyId()
        {
            using (var session = _dataStorage.CreateSession())
            {
                var company = session.Query<Customer>().FirstOrDefault(c => c.CompanyName == CompanyName);
                if (company != null)
                {
                    _companyId = company.Id;
                }
                else
                {
                    _companyId = null;
                }
            }
            //find Company Id
            if (!string.IsNullOrWhiteSpace(_companyId))
            {
                RefreshData();
            }
        }

        private void RefreshData()
        {
            PositionsList.Clear();
            using (var session = _dataStorage.CreateSession())
            {
                var baseQuery = session.Query<SaleConfirmation>();
                baseQuery = FilterByCustomerOrProvider(baseQuery);
                baseQuery = FilterMoreRecentThanFromDate(baseQuery);
                baseQuery = FIlterOlderThanToDate(baseQuery);
                var listOfPositions = baseQuery.Select(sc => new PositionsListRow
                {
                    CustomerName = sc.Customer.CompanyName,
                    DocumentDate = sc.DocumentDate,
                    DocumentId = sc.Id,
                    ProviderName = sc.Provider.CompanyName,
                    ShippingDate = sc.ShippingDate,
                }).OrderByDescending(sc => sc.DocumentDate);

                var results = listOfPositions.Take(100).ToList();
                CheckLoadingDocumentsExistence(session, results);
                CheckPriceConfirmationExistence(session, results);
                PositionsList.AddRange(results);
            }
        }

        private static void CheckPriceConfirmationExistence(IDocumentSession session, List<PositionsListRow> results)
        {
            var findPriceConfirmations =
                session.Load<PriceConfirmation>(results.Select(lop => "PriceConfirmations/" + lop.ProgressiveNumber)).ToList();
            foreach (var document in findPriceConfirmations)
            {
                if (document != null)
                {
                    var position = results.First(lop => lop.ProgressiveNumber == document.ProgressiveNumber);
                    position.HasPriceConfirmation = true;
                }
            }
        }

        private static void CheckLoadingDocumentsExistence(IDocumentSession session, List<PositionsListRow> results)
        {
            var findLoadingDocuments = session.Load<LoadingDocument>(results.Select(lop => "LoadingDocuments/" + lop.ProgressiveNumber)).ToList();
            foreach (var document in findLoadingDocuments)
            {
                if (document != null)
                {
                    var position = results.First(lop => lop.ProgressiveNumber == document.ProgressiveNumber);
                    position.HasLoadingDocument = true;
                }
            }
        }

        private IRavenQueryable<SaleConfirmation> FIlterOlderThanToDate(IRavenQueryable<SaleConfirmation> baseQuery)
        {
            if (_toDate.HasValue)
            {
                baseQuery = baseQuery.Where(sc => sc.DocumentDate <= _toDate);
            }
            return baseQuery;
        }

        private IRavenQueryable<SaleConfirmation> FilterMoreRecentThanFromDate(IRavenQueryable<SaleConfirmation> baseQuery)
        {
            if (_fromDate.HasValue)
            {
                baseQuery = baseQuery.Where(sc => sc.DocumentDate >= _fromDate);
            }
            return baseQuery;
        }

        private IRavenQueryable<SaleConfirmation> FilterByCustomerOrProvider(IRavenQueryable<SaleConfirmation> baseQuery)
        {
            if (!string.IsNullOrWhiteSpace(_companyId))
            {
                baseQuery = baseQuery.Where(sc => sc.Customer.Id == _companyId || sc.Provider.Id == _companyId);
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
        private string _companyId;
        private DateTime? _fromDate;
        private DateTime? _toDate;
        private PositionsListRow _selectedRow;
    }
}