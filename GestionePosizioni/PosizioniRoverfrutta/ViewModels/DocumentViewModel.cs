using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Models;
using QueryManager;
using QueryManager.Repositories;
using Raven.Abstractions.Extensions;

namespace PosizioniRoverfrutta.ViewModels
{
    internal class DocumentViewModel : INotifyPropertyChanged
    {
        public DocumentViewModel(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
            _saleConfirmationRepository = new SaleConfirmationRepository();
        }

        public int? DocumentId
        {
            get { return _saleConfirmation.Id == 0 ? (int?)null : _saleConfirmation.Id; }
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                        _saleConfirmationRepository.Save();
                    });
                }
                return saveCommand;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly ISaleConfirmationRepository _saleConfirmationRepository;

        private readonly IDataStorage _dataStorage;

        private SaleConfirmation _saleConfirmation;

        private ObservableCollection<ProductSold> _products;
    }
}
