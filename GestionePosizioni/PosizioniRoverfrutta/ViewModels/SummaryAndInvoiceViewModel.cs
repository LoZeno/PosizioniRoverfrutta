using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Practices.Prism;
using Models.Companies;
using Models.DocumentTypes;
using Models.Entities;
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

            using (var session = _dataStorage.CreateSession())
            {
                var defaults = session.Load<DefaultValues>(1);
                _summaryAndInvoice.InvoiceVat = defaults.InvoiceVat;
                _summaryAndInvoice.Witholding = defaults.Witholding;
            }

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

        public decimal InvoiceVat
        {
            get { return _summaryAndInvoice.InvoiceVat; }
            set
            {
                _summaryAndInvoice.InvoiceVat = value;
                OnPropertyChanged();
                UpdateDefaultValues();
                UpdateTotals();
            }
        }

        public decimal CalculatedInvoiceVat
        {
            get { return _summaryAndInvoice.CalculatedInvoiceVat; }
            set
            {
                _summaryAndInvoice.CalculatedInvoiceVat = value;
                OnPropertyChanged();
            }
        }

        public decimal TaxedAmount
        {
            get { return _summaryAndInvoice.TaxedAmount; }
            set
            {
                _summaryAndInvoice.TaxedAmount = value;
                OnPropertyChanged();
            }
        }

        public decimal Witholding
        {
            get { return _summaryAndInvoice.Witholding; }
            set
            {
                _summaryAndInvoice.Witholding = value;
                OnPropertyChanged();
                UpdateDefaultValues();
                UpdateTotals();
            }
        }

        public decimal CalculatedWitholding
        {
            get { return _summaryAndInvoice.CalculatedWitholding; }
            set
            {
                _summaryAndInvoice.CalculatedWitholding = value;
                OnPropertyChanged();
            }
        }

        public decimal NetAmount
        {
            get { return _summaryAndInvoice.NetAmount; }
            set
            {
                _summaryAndInvoice.NetAmount = value;
                OnPropertyChanged();
            }
        }

        private void UpdateDefaultValues()
        {
            using (var session = _dataStorage.CreateSession())
            {
                var defaultValues = session.Load<DefaultValues>(1);
                defaultValues.InvoiceVat = InvoiceVat;
                defaultValues.Witholding = Witholding;
                session.Store(defaultValues);
                session.SaveChanges();
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
                    var tempResults = session.Query<PriceConfirmation>()
                        .Where(
                            pc =>
                                pc.Customer.Id.Equals(_summaryAndInvoice.Customer.Id) &&
                                (StartDate <= pc.DocumentDate) && (pc.DocumentDate <= EndDate) &&
                                pc.CustomerCommission.HasValue)
                        .Select(pc =>pc).ToList();

                    var summaryRows = tempResults.Select(pc =>
                        new SummaryRowViewModel
                        {
                            DocumentId = pc.Id,
                            DocumentDate = pc.DocumentDate,
                            TransportDocument = pc.TransportDocument,
                            CompanyName = pc.Provider.CompanyName,
                            TaxableAmount = pc.TaxableAmount,
                            Commission = pc.CustomerCommission.Value
                        });

                    customerDataRows = summaryRows.ToList();

                    tempResults = session.Query<PriceConfirmation>()
                        .Where(
                            pc =>
                                pc.Provider.Id.Equals(_summaryAndInvoice.Customer.Id) &&
                                (StartDate <= pc.DocumentDate) && (pc.DocumentDate <= EndDate) &&
                                pc.ProviderCommission.HasValue)
                        .Select(pc => pc).ToList();

                    var providerSummaryRows = tempResults.Select(pc =>
                        new SummaryRowViewModel
                        {
                            DocumentId = pc.Id,
                            DocumentDate = pc.DocumentDate,
                            TransportDocument = pc.TransportDocument,
                            CompanyName = pc.Customer.CompanyName,
                            TaxableAmount = pc.TaxableAmount,
                            Commission = pc.ProviderCommission.Value
                        });

                    providerDataRows = providerSummaryRows.ToList(); 
                }

                var orderedList = new List<SummaryRowViewModel>();
                orderedList.AddRange(customerDataRows);
                orderedList.AddRange(providerDataRows);

                SummaryRows.Clear();
                SummaryRows.AddRange(orderedList.OrderBy(r => r.DocumentId));
            }
            else
            {
                SummaryRows.Clear();
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
            CommissionsTotal = Math.Round(SummaryRows.Sum(p => p.PayableAmount), 2);
            CalculatedInvoiceVat = Math.Round((CommissionsTotal*InvoiceVat/100), 2);
            TaxedAmount = CommissionsTotal + CalculatedInvoiceVat;
            CalculatedWitholding = Math.Round(((CommissionsTotal/2)*Witholding)/100, 2);
            NetAmount = TaxedAmount - Witholding;
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
