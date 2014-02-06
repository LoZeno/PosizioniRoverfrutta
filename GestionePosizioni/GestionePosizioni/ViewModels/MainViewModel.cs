using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Models;
using QueryManager.Repositories;
using Raven.Abstractions.Extensions;

namespace GestionePosizioni.ViewModels
{
    public class MainViewModel : IMainViewModel
    {
        private SaleConfirmation _saleConfirmation;
        private readonly ISaleConfirmationRepository _saleConfirmationRepository;
        private ICustomerRepository _providerRepository;
        private ObservableCollection<ProductSold> _products;

        public MainViewModel(SaleConfirmation saleConfirmation, ISaleConfirmationRepository saleConfirmationRepository)
        {
            _saleConfirmation = saleConfirmation;
            _products = new ObservableCollection<ProductSold>(_saleConfirmation.Products);
            _products.CollectionChanged += ProductsOnCollectionChanged;
            _saleConfirmationRepository = saleConfirmationRepository;
        }

        public MainViewModel(SaleConfirmation document, ISaleConfirmationRepository _repository, ICustomerRepository providerRepository)
            :this(document, _repository)
        {
            _providerRepository = providerRepository;
        }

        private void ProductsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            OnPropertyChanged("TotalPallets");
            OnPropertyChanged("TotalGrossWeight");
            OnPropertyChanged("TotalNetWeight");
            OnPropertyChanged("TotalPackages");
        }

        public int? DocumentId
        {
            get { return _saleConfirmation.Id == 0 ? (int?) null : _saleConfirmation.Id; }
            set
            {
                _saleConfirmation = value.HasValue
                    ? (_saleConfirmationRepository.FindById(value.Value) ?? new SaleConfirmation())
                    : new SaleConfirmation();
                _products.Clear();
                _products.AddRange(_saleConfirmation.Products);
                OnPropertyChanged("SaleConfirmationId");
                OnPropertyChanged("Customer");
                OnPropertyChanged("Provider");
                OnPropertyChanged("Products");
            }
        }

        public Customer Customer 
        {
            get
            {
                return _saleConfirmation.Customer;
            }
            set
            {
                _saleConfirmation.Customer = value;
                OnPropertyChanged("Customer");
            } 
        }
        public Customer Provider 
        {
            get
            {
                return _saleConfirmation.Provider;
            }
            set
            {
                _saleConfirmation.Provider = value;
                OnPropertyChanged("Provider");
            } 
        }

        public ObservableCollection<ProductSold> Products
        {
            get { return _products; }
        }

        public int TotalPallets 
        {
            get
            {
                return Products.Sum(x => x.Pallets);
            }
        }
        public decimal TotalGrossWeight
        {
            get { return Products.Sum(x => x.GrossWeight); }
        }

        public decimal TotalNetWeight
        {
            get { return Products.Sum(x => x.NetWeight); }
        }

        public int TotalPackages
        {
            get { return Products.Sum(x => x.Packages); }
        }

        private ICommand saveCommand;

        public ICommand Save
        {
            get
            {
                if (saveCommand == null)
                {
                    saveCommand = new DelegateCommand(delegate()
                    {
                        _saleConfirmation.Products = _products.ToList();
                        _saleConfirmationRepository.Add(_saleConfirmation);
                        _providerRepository.Add(_saleConfirmation.Customer);
                        _saleConfirmationRepository.Save();
                    });
                }
                return saveCommand;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
