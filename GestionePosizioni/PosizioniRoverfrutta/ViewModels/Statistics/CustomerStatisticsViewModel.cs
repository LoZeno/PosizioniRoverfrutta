using Models.Companies;
using Models.Entities;
using PosizioniRoverfrutta.Annotations;
using QueryManager;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using Models.DocumentTypes;
using QueryManager.Indexes;
using Raven.Client.Linq;
using System.Collections.Generic;
using System.Linq;

namespace PosizioniRoverfrutta.ViewModels.Statistics
{
    public class CustomerStatisticsViewModel : INotifyPropertyChanged
    {
        public CustomerStatisticsViewModel(IDataStorage dataStorage, string customerId)
        {
            ProductStatisticsRows = new ObservableCollection<ProductStatistics>();
            _dataStorage = dataStorage;
            using (var session = _dataStorage.CreateSession())
            {
                _customer = session.Load<Customer>(customerId);
            }
        }

        public string CustomerName
        {
            get { return _customer.CompanyName; }
        }

        public string Address
        {
            get { return _customer.Address; }
        }
        public string City
        {
            get { return _customer.City; }
        }
        public string Country
        {
            get { return _customer.Country; }
        }
        public string EmailAddress
        {
            get { return _customer.EmailAddress; }
        }
        public string PostCode
        {
            get { return _customer.PostCode; }
        }
        public string StateOrProvince
        {
            get { return _customer.StateOrProvince; }
        }
        public string VatCode
        {
            get { return _customer.VatCode; }
        }
        public DateTime? FromDate
        {
            get { return _fromDate; }
            set
            {
                _fromDate = value;
                OnPropertyChanged();
                UpdateProductRows();
            }
        }
        public DateTime? ToDate
        {
            get { return _toDate; }
            set
            {
                _toDate = value;
                OnPropertyChanged();
                UpdateProductRows();
            }
        }

        public StatisticsMode CustomerOrProvider
        {
            get { return _customerOrProvider; }
            set
            {
                _customerOrProvider = value;
                OnPropertyChanged();
                UpdateProductRows();
            }
        }

        public ObservableCollection<ProductStatistics> ProductStatisticsRows { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateProductRows()
        {
            ProductStatisticsRows.Clear();
            if (!_fromDate.HasValue || !_toDate.HasValue || _customer == null)
            {
                return;
            }
            using (var session = _dataStorage.CreateSession())
            {
                var temporaryData = new List<ProductStatistics>();
                var elements = session.Query<PriceConfirmation, PriceConfirmation_byCustomerIdAndProviderIdAndShippingDate>();
                elements = UpdateQueryBasedOnCustomerOrProviderChoice(elements)
                    .Where(x => x.ShippingDate >= _fromDate && x.ShippingDate <= _toDate);
                var enumerator = session.Advanced.Stream(elements);
                while(enumerator.MoveNext())
                {
                    var products = enumerator.Current.Document.ProductDetails;
                    foreach (var product in products)
                    {
                        if (temporaryData.Any(x => x.ProductId.Equals(product.ProductId)))
                        {
                            var single = temporaryData.Single(x => x.ProductId.Equals(product.ProductId));
                            single.Instances += 1;
                            single.NetWeight += product.NetWeight;
                            single.PriceSum += product.Price;
                            single.TotalAmount += product.TotalPrice;
                        }
                        else
                        {
                            temporaryData.Add(new ProductStatistics
                            {
                                ProductId = product.ProductId,
                                PriceSum = product.Price,
                                Instances = 1,
                                Description = product.Description,
                                NetWeight = product.NetWeight,
                                TotalAmount = product.TotalPrice
                            });
                        }
                    }
                }
                ProductStatisticsRows = new ObservableCollection<ProductStatistics>(temporaryData.OrderBy(x => x.Description).ToList());
            }
        }

        private IRavenQueryable<PriceConfirmation> UpdateQueryBasedOnCustomerOrProviderChoice(IRavenQueryable<PriceConfirmation> query)
        {
            if (_customerOrProvider == StatisticsMode.Provider)
            {
                return query.Where(x => x.Provider.Id.Equals(_customer.Id));
            }
            return query.Where(x => x.Customer.Id.Equals(_customer.Id));
        }

        private Customer _customer;
        private IDataStorage _dataStorage;
        private DateTime? _fromDate;
        private DateTime? _toDate;
        private StatisticsMode _customerOrProvider;
    }

    public enum StatisticsMode
    {
        Customer,
        Provider
    }
}
