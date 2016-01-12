using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Models.DocumentTypes;
using Models.Entities;
using PosizioniRoverfrutta.Annotations;
using PosizioniRoverfrutta.Windows;
using QueryManager;
using QueryManager.Indexes;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Raven.Client.Linq;

namespace PosizioniRoverfrutta.ViewModels
{
    public class ProductsWindowGridViewModel : INotifyPropertyChanged
    {
        public ProductsWindowGridViewModel(IDataStorage dataStorage, IWindowManager windowManager)
        {
            _dataStorage = dataStorage;
            _windowManager = windowManager;
            ProductsList = new ObservableCollection<ProductRow>();

            LoadAllData();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string SearchBox
        {
            get { return _searchBox; }
            set
            {
                _searchBox = value;
                LoadAllData();
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get { return _selectedProduct?.Description; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    SaveButtonEnabled = false;
                _selectedProduct.Description = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ProductRow> ProductsList { get; set; }

        public bool SaveButtonEnabled { get; private set; }

        public bool DeleteButtonEnabled { get; private set; }

        public bool EditControlsEnabled { get { return _selectedProduct != null; } }

        public ICommand NextPage
        {
            get { return nextPageCommand ?? (nextPageCommand = new DelegateCommand(IncreaseSkip)); }
        }

        public ICommand PreviousPage
        {
            get { return previousPageCommand ?? (previousPageCommand = new DelegateCommand(DecreaseSkip)); }
        }

        public ICommand Refresh
        {
            get { return refreshCommand ?? (refreshCommand = new DelegateCommand(LoadAllData)); }
        }

        public ICommand Save
        {
            get { return saveCommand ?? (saveCommand = new DelegateCommand(SaveAndRefresh)); }
        }

        public ICommand CreateNew
        {
            get { return createCommand ?? (createCommand = new DelegateCommand(CreateNewProduct)); }
        }

        public ICommand DeleteProduct
        {
            get { return deleteCommand ?? (deleteCommand = new DelegateCommand(DeleteSelectedProduct)); }
        }

        public void LoadSelectedProduct(int? selectedProductId)
        {
            if (selectedProductId.HasValue)
            {
                using (var session = _dataStorage.CreateSession())
                {
                    _selectedProduct = session.Load<ProductDescription>(selectedProductId);
                    OnPropertyChanged("EditControlsEnabled");
                }
            }
            else
            {
                _selectedProduct = null;
                OnPropertyChanged("EditControlsEnabled");
            }

            OnPropertyChanged("Description");
            SetActionButtonsState(selectedProductId);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (!propertyName.In("SaveButtonEnabled", "DeleteButtonEnabled", "SelectedProduct", "SearchBox"))
            {
                if (!SaveButtonEnabled && (!string.IsNullOrWhiteSpace(_selectedProduct?.Description)))
                {
                    SaveButtonEnabled = true;
                    OnPropertyChanged("SaveButtonEnabled");
                }
            }
        }

        private void LoadAllData()
        {
            ProductsList.Clear();
            using (var session = _dataStorage.CreateSession())
            {
                if (string.IsNullOrWhiteSpace(SearchBox))
                {
                    ProductsList.AddRange(session.Query<ProductRow, ProductsWithNumberOfDocuments>().OrderBy(c => c.Description).Skip(skipPositions).Take(100).ToList());
                }
                else
                {
                    var customersQuery = session.Query<ProductRow, ProductsWithNumberOfDocuments>();
                    var queryByName = SearchBox.Split(' ').Aggregate(customersQuery, (current, term) => current.Search(c => c.Description, "*" + term + "*", options: SearchOptions.And, escapeQueryOptions: EscapeQueryOptions.AllowAllWildcards));
                    ProductsList.AddRange(queryByName.OrderBy(c => c.Description).Take(100).ToList());
                }
            }
            if (ProductsList.Count == 0)
            {
                DecreaseSkip();
            }
            var selectedProductId = _selectedProduct?.Id;
            LoadSelectedProduct(selectedProductId);
        }

        private void SetActionButtonsState(int? selectedProductId)
        {
            DeleteButtonEnabled = _selectedProduct != null;
            using (var session = _dataStorage.CreateSession())
            {
                if (session.Query<PriceConfirmation>().Count(pc => pc.ProductDetails.Any(pd => pd.ProductId == selectedProductId)) > 0
                    || session.Query<LoadingDocument>().Count(ld => ld.ProductDetails.Any(pd => pd.ProductId == selectedProductId)) > 0
                    || session.Query<SaleConfirmation>().Count(sc => sc.ProductDetails.Any(pd => pd.ProductId == selectedProductId)) > 0)
                    DeleteButtonEnabled = false;
            }
            SaveButtonEnabled = false;
            OnPropertyChanged("DeleteButtonEnabled");
            OnPropertyChanged("SaveButtonEnabled");
        }

        private void SaveAndRefresh()
        {
            string description = Description;
            SaveSelectedProduct();
            LoadSelectedProduct(null);
            LoadAllData();
            _windowManager.PopupMessage(string.Format("Prodotto {0} salvato correttamente", description), "Prodotto salvato");
        }

        private void SaveSelectedProduct()
        {
            using (var session = _dataStorage.CreateSession())
            {
                session.Store(_selectedProduct);
                session.SaveChanges();
            }
        }

        private void CreateNewProduct()
        {
            _selectedProduct = new ProductDescription();
            OnPropertyChanged("EditControlsEnabled");
            OnPropertyChanged("Description");
            DeleteButtonEnabled = false;
            OnPropertyChanged("DeleteButtonEnabled");
        }

        private void DeleteSelectedProduct()
        {
            string description = _selectedProduct.Description;
            using (var session = _dataStorage.CreateSession())
            {
                var itemToDelete = session.Load<ProductDescription>(_selectedProduct.Id);
                session.Delete<ProductDescription>(itemToDelete);
                session.SaveChanges();
            }
            LoadAllData();
            _windowManager.PopupMessage(string.Format("Prodotto {0} cancellato dal database", description), "Prodotto eliminato");
        }

        private void IncreaseSkip()
        {
            if (ProductsList.Count == 100)
            {
                skipPositions += 100;
                LoadAllData();
            }
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
                LoadAllData();
            }
        }

        private readonly IDataStorage _dataStorage;
        private readonly IWindowManager _windowManager;
        private string _searchBox;
        private int skipPositions = 0;
        private ProductDescription _selectedProduct;
        private ICommand nextPageCommand;
        private ICommand previousPageCommand;
        private ICommand refreshCommand;
        private ICommand saveCommand;
        private ICommand createCommand;
        private ICommand deleteCommand;
    }
}
