using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Models.Entities;
using PosizioniRoverfrutta.Annotations;
using PosizioniRoverfrutta.ViewModels.BaseClasses;

namespace PosizioniRoverfrutta.ViewModels
{
    public class SummaryRowViewModel : EditableModelBase<SummaryRowViewModel>, INotifyPropertyChanged
    {
        public SummaryRowViewModel() : this(new SummaryRow())
        {
            
        }

        public SummaryRowViewModel(SummaryRow summaryRow)
        {
            _summaryRow = summaryRow;
            UpdateTotal();
        }

        public SummaryRow SummaryRow
        {
            get { return _summaryRow; }
            set
            {
                _summaryRow = value;
                OnPropertyChanged();
                OnPropertyChanged("DocumentId");
                OnPropertyChanged("ShippingDate");
                OnPropertyChanged("TransportDocument");
                OnPropertyChanged("CompanyName");
                OnPropertyChanged("TaxableAmount");
                OnPropertyChanged("Commission");
                OnPropertyChanged("PayableAmount");
            }
        }

        public int DocumentId
        {
            get { return _summaryRow.DocumentId; }
            set
            {
                _summaryRow.DocumentId = value;
                OnPropertyChanged();
            }
        }

        public DateTime? ShippingDate 
        { 
            get { return _summaryRow.ShippingDate; }
            set
            {
                _summaryRow.ShippingDate = value;
                OnPropertyChanged();
            }
        }
        public string TransportDocument 
        { 
            get { return _summaryRow.TransportDocument; }
            set
            {
                _summaryRow.TransportDocument = value;
                OnPropertyChanged();
            }
        }
        public string CompanyName 
        {
            get { return _summaryRow.CompanyName; }
            set
            {
                _summaryRow.CompanyName = value;
                OnPropertyChanged();
            }
        }

        public decimal TaxableAmount
        {
            get { return _summaryRow.TaxableAmount; }
            set
            {
                _summaryRow.TaxableAmount = value;
                OnPropertyChanged();
                UpdateTotal();
            }
        }

        public decimal Commission
        {
            get { return _summaryRow.Commission; }
            set
            {
                _summaryRow.Commission = value;
                OnPropertyChanged();
                UpdateTotal();
            }
        }

        public decimal PayableAmount
        {
            get { return _summaryRow.PayableAmount; }
            set
            {
                _summaryRow.PayableAmount = value;
                OnPropertyChanged();
            }
        }

        private void UpdateTotal()
        {
            PayableAmount = Math.Round((TaxableAmount*Commission)/100, 2);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private SummaryRow _summaryRow;
    }
}