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
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using CustomWPFControls;
using Microsoft.Practices.Prism;

namespace PosizioniRoverfrutta.ViewModels.Statistics
{
    public class CustomerStatisticsViewModel : INotifyPropertyChanged
    {
        public CustomerStatisticsViewModel(IDataStorage dataStorage, string customerId)
        {
            ProductStatisticsRows = new ObservableCollection<ProductStatistics>();
            CathegoryStatisticsRows = new ObservableCollection<ProductStatistics>();
            CathegoryNamesProvider = new AutocompleteCathegoryNamesProvider(_productsPerCathegory);
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
                ClearCathegories();
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
                ClearCathegories();
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
                ClearCathegories();
            }
        }
        public string Cathegory
        {
            get { return _cathegory; }
            set
            {
                _cathegory = value;
                OnPropertyChanged();
            }
        }
        public IList<ProductStatistics> SelectedProductRows
        {
            get { return _selectedProductRows; }
            set
            {
                _selectedProductRows = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ProductStatistics> ProductStatisticsRows { get; private set; }
        public ObservableCollection<ProductStatistics> CathegoryStatisticsRows { get; private set; }

        public IAutoCompleteBoxDataProvider CathegoryNamesProvider { get; private set; }

        public ICommand AddToCathegory
        {
            get { return addToCathegoryCommand ?? (addToCathegoryCommand = new DelegateCommand(AddSelectedRowsToCathegory)); }
        }

        public ICommand RemoveCathegory
        {
            get { return removeFromCathegories ?? (removeFromCathegories = new DelegateCommand(RemoveSelectedCathegory)); }
        }

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
                            single.MinimumPrice = Math.Min(single.MinimumPrice, product.Price);
                            single.MaximumPrice = Math.Max(single.MaximumPrice, product.Price);
                            single.TotalAmount += product.TotalPrice;
                        }
                        else
                        {
                            temporaryData.Add(new ProductStatistics
                            {
                                ProductId = product.ProductId,
                                MinimumPrice = product.Price,
                                MaximumPrice = product.Price,
                                PriceSum = product.Price,
                                Instances = 1,
                                Description = product.Description,
                                NetWeight = product.NetWeight,
                                TotalAmount = product.TotalPrice
                            });
                        }
                    }
                }
                ProductStatisticsRows.AddRange(temporaryData.OrderBy(x => x.Description).ToList());
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

        private void AddSelectedRowsToCathegory()
        {
            if (_selectedProductRows == null || string.IsNullOrWhiteSpace(_cathegory))
            {
                return;
            }
            var statisticsRows = CathegoryStatisticsRows.ToList();
            if (!_productsPerCathegory.ContainsKey(_cathegory))
            {
                _productsPerCathegory.Add(_cathegory, new List<string>());
                statisticsRows.Add(new ProductStatistics { Description = _cathegory });
            }
            var existingCathegory = _productsPerCathegory[_cathegory];
            var cathegory = statisticsRows.Single(x => x.Description.Equals(_cathegory));
            foreach (var productRow in _selectedProductRows)
            {
                if (existingCathegory.Contains(productRow.Description))
                {
                    break;
                }
                cathegory.Instances += productRow.Instances;
                cathegory.NetWeight += productRow.NetWeight;
                cathegory.PriceSum += productRow.PriceSum;
                cathegory.MaximumPrice = Math.Max(cathegory.MaximumPrice, productRow.MaximumPrice);
                cathegory.MinimumPrice = cathegory.MinimumPrice == 0 ? productRow.MinimumPrice : Math.Min(cathegory.MinimumPrice, productRow.MinimumPrice);
                cathegory.TotalAmount += productRow.TotalAmount;
                existingCathegory.Add(productRow.Description);
            }
            CathegoryStatisticsRows.Clear();
            CathegoryStatisticsRows.AddRange(statisticsRows.OrderBy(x => x.Description).ToList());
        }

        private void RemoveSelectedCathegory()
        {
            if (_productsPerCathegory.ContainsKey(_cathegory))
            {
                var cathegoryToRemove = CathegoryStatisticsRows.Single(x => x.Description.Equals(_cathegory));
                CathegoryStatisticsRows.Remove(cathegoryToRemove);
                _productsPerCathegory.Remove(_cathegory);
            }
        }

        private void ClearCathegories()
        {
            CathegoryStatisticsRows.Clear();
            _productsPerCathegory.Clear();
        }

        private Customer _customer;
        private IDataStorage _dataStorage;
        private DateTime? _fromDate;
        private DateTime? _toDate;
        private StatisticsMode _customerOrProvider;
        private string _cathegory;
        private IList<ProductStatistics> _selectedProductRows;
        private ICommand addToCathegoryCommand;
        private ICommand removeFromCathegories;
        private Dictionary<string, List<string>> _productsPerCathegory = new Dictionary<string, List<string>>();

        private class AutocompleteCathegoryNamesProvider : IAutoCompleteBoxDataProvider
        {
            public AutocompleteCathegoryNamesProvider(Dictionary<string, List<string>> productsPerCathegory)
            {
                _source = productsPerCathegory;
            }
            public IEnumerable<string> GetItems(string textPattern)
            {
                if (string.IsNullOrWhiteSpace(textPattern))
                    return _source.Keys;

                return _source.Keys.Where(x => x.ToLowerInvariant().StartsWith(textPattern.ToLowerInvariant()));
            }

            private Dictionary<string, List<string>> _source;
        }
    }

    public enum StatisticsMode
    {
        Customer,
        Provider
    }
}
