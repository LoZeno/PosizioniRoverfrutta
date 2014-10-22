using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Practices.Prism;
using Models.Companies;
using Models.DocumentTypes;
using PosizioniRoverfrutta.Annotations;
using QueryManager;

namespace PosizioniRoverfrutta.ViewModels
{
    public class SummaryAndInvoiceViewModel : INotifyPropertyChanged
    {
        public SummaryAndInvoiceViewModel(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;

            _summaryAndInvoice = new SummaryAndInvoice();

            SummaryRows = new ObservableCollection<SummaryRowViewModel>();
            SummaryRows.CollectionChanged += SummaryRows_CollectionChanged;
        }

        public string CustomerName
        {
            get { return _summaryAndInvoice.Customer == null ? string.Empty : _summaryAndInvoice.Customer.CompanyName; }
            set
            {
                FindCustomer(value);
                OnPropertyChanged();
                UpdateRows();
            }
        }

        public DateTime? StartDate
        {
            get { return _summaryAndInvoice.StartDate; }
            set
            {
                _summaryAndInvoice.StartDate = value;
                OnPropertyChanged();
                UpdateRows();
            }
        }

        public DateTime? EndDate
        {
            get { return _summaryAndInvoice.EndDate; }
            set
            {
                _summaryAndInvoice.EndDate = value;
                OnPropertyChanged();
                UpdateRows();
            }
        }

        public decimal CommissionsTotal
        {
            get { return _summaryAndInvoice.CommissionsTotal; }
            set
            {
                _summaryAndInvoice.CommissionsTotal = value;
                OnPropertyChanged();
            }
        }

        private void UpdateRows()
        {
            if (_summaryAndInvoice.Customer != null && _summaryAndInvoice.StartDate != null &&
                _summaryAndInvoice.EndDate != null)
            {
                List<SummaryRowViewModel> customerDataRows;
                List<SummaryRowViewModel> providerDataRows;
                using (var session = _dataStorage.CreateSession())
                {
                    customerDataRows =
                        session.Query<PriceConfirmation>()
                            .Where(
                                pc =>
                                    pc.Customer.Id.Equals(_summaryAndInvoice.Customer.Id) &&
                                    (StartDate <= pc.DocumentDate) && (pc.DocumentDate <= EndDate) && pc.CustomerCommission.HasValue)
                            .Select(pc => new SummaryRowViewModel {DocumentId = pc.Id, DocumentDate = pc.DocumentDate, TransportDocument = pc.TransportDocument, CompanyName = pc.Provider.CompanyName, TaxableAmount = pc.TaxableAmount, Commission = pc.CustomerCommission.Value }).ToList();

                    providerDataRows = 
                        session.Query<PriceConfirmation>()
                            .Where(
                                pc =>
                                    pc.Provider.Id.Equals(_summaryAndInvoice.Customer.Id) &&
                                    (StartDate <= pc.DocumentDate) && (pc.DocumentDate <= EndDate) && pc.ProviderCommission.HasValue)
                            .Select(pc => new SummaryRowViewModel { DocumentId = pc.Id, DocumentDate = pc.DocumentDate, TransportDocument = pc.TransportDocument, CompanyName = pc.Customer.CompanyName, TaxableAmount = pc.TaxableAmount, Commission = pc.ProviderCommission.Value }).ToList(); 
                }

                var orderedList = new List<SummaryRowViewModel>();
                orderedList.AddRange(customerDataRows);
                orderedList.AddRange(providerDataRows);

                SummaryRows.Clear();
                SummaryRows.AddRange(orderedList.OrderBy(r => r.DocumentId));
            }
            UpdateTotals();
        }

        private void FindCustomer(string companyName)
        {
            using (var session = _dataStorage.CreateSession())
            {
                _summaryAndInvoice.Customer = session.Query<Customer>().FirstOrDefault(c => c.CompanyName == companyName);
            }
        }

        public ObservableCollection<SummaryRowViewModel> SummaryRows { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void UpdateTotals()
        {
            CommissionsTotal = SummaryRows.Sum(p => p.PayableAmount);
        }

        void SummaryRows_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (SummaryRowViewModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (SummaryRowViewModel item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateTotals();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly IDataStorage _dataStorage;
        private SummaryAndInvoice _summaryAndInvoice;
    }
}
