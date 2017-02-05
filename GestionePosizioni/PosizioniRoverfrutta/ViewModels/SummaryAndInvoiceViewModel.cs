using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using PosizioniRoverfrutta.Reports;
using PosizioniRoverfrutta.Services;
using PosizioniRoverfrutta.Windows;
using QueryManager;
using QueryManager.Indexes;
using Raven.Client;

namespace PosizioniRoverfrutta.ViewModels
{
    public class SummaryAndInvoiceViewModel : INotifyPropertyChanged
    {
        public SummaryAndInvoiceViewModel(IDataStorage dataStorage, IWindowManager windowManager)
        {
            _dataStorage = dataStorage;
            _windowManager = windowManager;

            _summaryAndInvoice = new SummaryAndInvoice();

            using (var session = _dataStorage.CreateSession())
            {
                var defaults = session.Load<DefaultValues>(1);
                if (defaults != null)
                {
                    _summaryAndInvoice.InvoiceVat = defaults.InvoiceVat;
                    _summaryAndInvoice.Witholding = defaults.Witholding;
                }
            }

            SummaryRows = new ObservableCollection<SummaryRowViewModel>();
            SummaryRows.CollectionChanged += SummaryRows_CollectionChanged;
            PartialsByCompanyName = new ObservableCollection<PartialByCompanyName>();
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

        public bool IncludeOpenPositions
        {
            get { return _summaryAndInvoice.IncludeOpenPositions; }
            set
            {
                _summaryAndInvoice.IncludeOpenPositions = value;
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

        public int? InvoiceNumber
        {
            get { return _summaryAndInvoice.InvoiceNumber; }
            set
            {
                _summaryAndInvoice.InvoiceNumber = value;
                OnPropertyChanged();
            }
        }

        public DateTime? InvoiceDate
        {
            get { return _summaryAndInvoice.InvoiceDate; }
            set
            {
                _summaryAndInvoice.InvoiceDate = value;
                OnPropertyChanged();
            }
        }

        public string Status
        {
            get { return _status; }
            private set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public bool ShowVatArea
        {
            get
            {
                if (_summaryAndInvoice.Customer == null)
                    return true;
                return !_summaryAndInvoice.Customer.DoNotApplyVat;
            }
        }

        public ObservableCollection<SummaryRowViewModel> SummaryRows { get; private set; }

        public ICommand PrintInvoice
        {
            get { return printInvoice ?? (printInvoice = new DelegateCommand(PrintInvoiceCommand())); }
        }

        public ICommand PrintSummary
        {
            get { return printSummmary ?? (printSummmary = new DelegateCommand(PrintSummaryDocument())); }
        }

        public ObservableCollection<PartialByCompanyName> PartialsByCompanyName { get; private set; }

        private Action PrintInvoiceCommand()
        {
            return delegate
            {
                var path = OpenSavePdfDialog("Fattura");
                if (string.IsNullOrWhiteSpace(path))
                {
                    Status = "Creazione del PDF annullata";
                    return;
                }
                _summaryAndInvoice.Base64Logo = ResourceHelpers.LoadBase64Logo();
                var report = new InvoiceReport(_summaryAndInvoice, path);
                report.CreatePdf();
                Status = string.Format("PDF della Fattura creato correttamente");
            };
        }

        private Action PrintSummaryDocument()
        {
            return delegate
            {
                var path = OpenSavePdfDialog("Riepilogo");
                if (string.IsNullOrWhiteSpace(path))
                {
                    Status = "Creazione del PDF annullata";
                    return;
                }
                _summaryAndInvoice.Base64Logo = ResourceHelpers.LoadBase64Logo();
                RefreshSummaryRowsInModel();

                RefreshPartialsByCompanyNameInModel();

                var report = new SummaryReport(_summaryAndInvoice, path);
                report.CreatePdf();
                Status = string.Format("PDF del Documento creato correttamente");
            };
        }

        private void RefreshPartialsByCompanyNameInModel()
        {
            _summaryAndInvoice.PartialsByCompanyName.Clear();
            foreach (var partialByCompanyName in PartialsByCompanyName)
            {
                _summaryAndInvoice.PartialsByCompanyName.Add(partialByCompanyName);
            }
        }

        private void RefreshSummaryRowsInModel()
        {
            _summaryAndInvoice.SummaryRows.Clear();
            foreach (var summaryRowViewModel in SummaryRows)
            {
                if (summaryRowViewModel.CanMakeInvoice)
                    _summaryAndInvoice.SummaryRows.Add(summaryRowViewModel.SummaryRow);
            }
        }

        private string OpenSavePdfDialog(string filePrefix)
        {
            return _windowManager.OpenSaveToPdfDialog(string.Format("{0}-{1}-{2}-{3}", filePrefix, CustomerName, GetStartDateString(), GetEndDateString()));
        }

        private string GetEndDateString()
        {
            return EndDate.HasValue ? EndDate.Value.ToString("yyyyMMdd") : "0000";
        }

        private string GetStartDateString()
        {
            return StartDate.HasValue ? StartDate.Value.ToString("yyyyMMdd") : "0000";
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
            SummaryRows.Clear();
            if (_summaryAndInvoice.Customer != null && _summaryAndInvoice.StartDate != null &&
                _summaryAndInvoice.EndDate != null)
            {

                var orderedList = new List<SummaryRowViewModel>();
                using (var session = _dataStorage.CreateSession())
                {
                    LoadPriceConfirmations(orderedList, session);

                    if (_summaryAndInvoice.IncludeOpenPositions)
                    {
                        LoadLoadingDocuments(orderedList, session);

                        LoadSalesConfirmations(orderedList, session);
                    }
                }
                SummaryRows.AddRange(orderedList.OrderBy(r => r.DocumentId));
            }
            UpdateTotals();
            UpdatePartialsByCompanyName();
            OnPropertyChanged("ShowVatArea");
        }

        private void UpdatePartialsByCompanyName()
        {
            PartialsByCompanyName.Clear();
            var partialResults = SummaryRows
                .GroupBy(summaryRow => summaryRow.CompanyName)
                .Select(group => new PartialByCompanyName
                {
                    CompanyName = group.Key,
                    Total = group.Sum(row => row.PayableAmount)
                })
                .OrderBy(row => row.CompanyName)
                .ToList();
            PartialsByCompanyName.AddRange(partialResults);
        }

        private void LoadSalesConfirmations(List<SummaryRowViewModel> orderedList, IDocumentSession session)
        {
            var currentIds = orderedList.Select(el => el.DocumentId).ToArray();
            var openRows = session.Query<SummaryRow, SummaryOfOpenPosition>()
                .Where(sr => sr.InvoiceCustomerId.Equals(_summaryAndInvoice.Customer.Id)
                             && (StartDate <= sr.ShippingDate) && (sr.ShippingDate <= EndDate));

            var enumerator = session.Advanced.Stream(openRows);
            while (enumerator.MoveNext())
            {
                var row = enumerator.Current.Document;
                if(!currentIds.Contains(row.DocumentId))
                {
                    orderedList.Add(new SummaryRowViewModel(row));
                }
            }
        }

        private void LoadLoadingDocuments(List<SummaryRowViewModel> orderedList, IDocumentSession session)
        {
            var currentIds = orderedList.Select(el => el.DocumentId).ToArray();
            var newRows = session.Query<SummaryRow, SummaryOfPartialPositions>()
                .Where(sr => sr.InvoiceCustomerId.Equals(_summaryAndInvoice.Customer.Id)
                             && (StartDate <= sr.ShippingDate) && (sr.ShippingDate <= EndDate));

            var enumerator = session.Advanced.Stream(newRows);
            while(enumerator.MoveNext())
            {
                var row = enumerator.Current.Document;
                if (!currentIds.Contains(row.DocumentId))
                {
                    orderedList.Add(new SummaryRowViewModel(row));
                }
            }
        }

        private void LoadPriceConfirmations(List<SummaryRowViewModel> orderedList, IDocumentSession session)
        {
            var elements = session.Query<SummaryRow, SummaryOfPositions>()
                .Where(sr => sr.InvoiceCustomerId.Equals(_summaryAndInvoice.Customer.Id)
                             && (StartDate <= sr.ShippingDate) && (sr.ShippingDate <= EndDate));

            var enumerator = session.Advanced.Stream(elements);
            while (enumerator.MoveNext())
            {
                orderedList.Add(new SummaryRowViewModel(enumerator.Current.Document));
            }
        }

        private void FindCustomer(string companyName)
        {
            using (var session = _dataStorage.CreateSession())
            {
                _summaryAndInvoice.Customer = session.Query<Customer>().FirstOrDefault(c => c.CompanyName == companyName);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void UpdateTotals()
        {
            CommissionsTotal = Math.Round(SummaryRows.Where(r => r.CanMakeInvoice).Sum(p => p.PayableAmount), 2);
            CalculatedInvoiceVat = 0;
            CalculatedWitholding = 0;
            if (_summaryAndInvoice.Customer != null && !_summaryAndInvoice.Customer.DoNotApplyVat)
            {
                CalculatedInvoiceVat = (CommissionsTotal*InvoiceVat/100).RoundUp(2);
                CalculatedWitholding = Math.Round(((CommissionsTotal / 2) * Witholding) / 100, 2);
            }
            TaxedAmount = CommissionsTotal + CalculatedInvoiceVat;
            NetAmount = TaxedAmount - CalculatedWitholding;
        }

        void SummaryRows_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        private readonly IWindowManager _windowManager;

        private SummaryAndInvoice _summaryAndInvoice;

        private ICommand printSummmary;

        private string _status;

        private ICommand printInvoice;

    }
}
