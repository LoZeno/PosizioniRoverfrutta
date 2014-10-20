using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Models;
using Models.DocumentTypes;
using PosizioniRoverfrutta.Annotations;
using PosizioniRoverfrutta.Reports;
using PosizioniRoverfrutta.Windows;
using QueryManager;
using Raven.Client;

namespace PosizioniRoverfrutta.ViewModels
{
    public class LoadingDocumentViewModel : INotifyPropertyChanged
    {
        public LoadingDocumentViewModel(IDataStorage dataStorage, IWindowManager windowManager)
        {
            _dataStorage = dataStorage;
            _windowManager = windowManager;
            CompanyControlViewModel = new CompanyControlViewModel<Customer>(_dataStorage);
            ProviderControlViewModel = new CompanyControlViewModel<Customer>(_dataStorage);
            TransporterControlViewModel = new CompanyControlViewModel<Transporter>(_dataStorage);

            LoadingDocument = new LoadingDocument();
            ProductDetails = new ObservableCollection<ProductRowViewModel>();
            ProductDetails.CollectionChanged += ProductDetails_CollectionChanged;
        }

        public int Id
        {
            get { return LoadingDocument.Id; }
            set
            {
                LoadDocument(value);

                OnPropertyChanged();
                OnPropertyChanged("LoadingDocument");
                OnPropertyChanged("DocumentDate");
                OnPropertyChanged("ShippingDate");
                OnPropertyChanged("TransportDocument");
                OnPropertyChanged("TruckLicensePlate");
                OnPropertyChanged("Rental");
                OnPropertyChanged("DeliveryEx");
                OnPropertyChanged("TermsOfPayment");
                OnPropertyChanged("InvoiceDiscount");
                OnPropertyChanged("CustomerCommission");
                OnPropertyChanged("ProviderCommission");
                OnPropertyChanged("Notes");
                OnPropertyChanged("Lot");
                OnPropertyChanged("OrderCode");
                OnPropertyChanged("EnableButtons");
            }
        }

        public DateTime? DocumentDate
        {
            get { return LoadingDocument.DocumentDate; }
            set
            {
                LoadingDocument.DocumentDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime? ShippingDate
        {
            get { return LoadingDocument.ShippingDate; }
            set
            {
                LoadingDocument.ShippingDate = value;
                OnPropertyChanged();
            }
        }

        public string TransportDocument
        {
            get { return LoadingDocument.TransportDocument; }
            set
            {
                LoadingDocument.TransportDocument = value;
                OnPropertyChanged();
            }
        }

        public string TruckLicensePlate
        {
            get { return LoadingDocument.TruckLicensePlate; }
            set
            {
                LoadingDocument.TruckLicensePlate = value;
                OnPropertyChanged();
            }
        }

        public decimal? Rental
        {
            get { return LoadingDocument.Rental; }
            set
            {
                LoadingDocument.Rental = value;
                OnPropertyChanged();
            }
        }

        public string DeliveryEx
        {
            get { return LoadingDocument.DeliveryEx; }
            set
            {
                LoadingDocument.DeliveryEx = value;
                OnPropertyChanged();
            }
        }

        public string TermsOfPayment
        {
            get { return LoadingDocument.TermsOfPayment; }
            set
            {
                LoadingDocument.TermsOfPayment = value;
                OnPropertyChanged();
            }
        }

        public decimal? InvoiceDiscount
        {
            get { return LoadingDocument.InvoiceDiscount; }
            set
            {
                LoadingDocument.InvoiceDiscount = value;
                OnPropertyChanged();
            }
        }

        public decimal? CustomerCommission
        {
            get { return LoadingDocument.CustomerCommission; }
            set
            {
                LoadingDocument.CustomerCommission = value;
                OnPropertyChanged();
            }
        }


        public decimal? ProviderCommission
        {
            get { return LoadingDocument.ProviderCommission; }
            set
            {
                LoadingDocument.ProviderCommission = value;
                OnPropertyChanged();
            }
        }

        public string Notes
        {
            get { return LoadingDocument.Notes; }
            set
            {
                LoadingDocument.Notes = value;
                OnPropertyChanged();
            }

        }
        public string Lot
        {
            get { return LoadingDocument.Lot; }
            set
            {
                LoadingDocument.Lot = value;
                OnPropertyChanged();
            }

        }
        public string OrderCode
        {
            get { return LoadingDocument.OrderCode; }
            set
            {
                LoadingDocument.OrderCode = value;
                OnPropertyChanged();
            }

        }

        public int TotalPallets
        {
            get { return ProductDetails.Sum(p => p.Pallets); }
        }

        public int TotalPackages
        {
            get { return ProductDetails.Sum(p => p.Packages); }
        }

        public decimal TotalGross
        {
            get { return ProductDetails.Sum(p => p.GrossWeight); }
        }

        public decimal TotalNet
        {
            get { return ProductDetails.Sum(p => p.NetWeight); }
        }

        public decimal TotalAmount
        {
            get { return ProductDetails.Sum(p => p.TotalPrice); }
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

        public bool EnableButtons
        {
            get { return Id != -1; }
        }

        public CompanyControlViewModel<Customer> CompanyControlViewModel { get; private set; }

        public CompanyControlViewModel<Customer> ProviderControlViewModel { get; private set; }

        public CompanyControlViewModel<Transporter> TransporterControlViewModel { get; private set; }

        public ObservableCollection<ProductRowViewModel> ProductDetails { get; private set; }

        public ICommand Reload
        {
            get { return reloadCommand ?? (reloadCommand = new DelegateCommand(ReloadAction())); }
        }

        public LoadingDocument LoadingDocument { get; set; }

        public ICommand SaveAll
        {
            get
            {
                return saveAllCommand ?? (saveAllCommand = new DelegateCommand(SaveDocumentAction()));
            }
        }

        public ICommand Print
        {
            get { return printDocument ?? (printDocument = new DelegateCommand(PrintDocument())); }
        }

        public ICommand Convert
        {
            get { return convertDocument ?? (convertDocument = new DelegateCommand(ConvertDocument())); }
        }

        private Action ConvertDocument()
        {
            return delegate
            {
                SaveAllData();
                _windowManager.InstantiateWindow(Id.ToString(), WindowTypes.DistintaCarico);
            };
        }

        private Action PrintDocument()
        {
            return delegate
            {
                SaveAllData();
                var path = _windowManager.OpenSaveToPdfDialog(string.Format("DistintaCarico-{0}", Id));
                if (string.IsNullOrWhiteSpace(path))
                {
                    Status = "Creazione del PDF annullata";
                }
                else
                {
                    var report = new LoadingDocumentReport(LoadingDocument, path);
                    report.CreatePdf();
                    Status = string.Format("PDF del Documento n° {0} creato correttamente", Id);
                }
            };
        }

        private void LoadDocument(int value)
        {
            LoadingDocument loadingDocument = null;
            using (var session = _dataStorage.CreateSession())
            {
                loadingDocument = session.Load<LoadingDocument>(value);
            
                if (loadingDocument == null)
                {
                    var saleconfirmation = session.Load<SaleConfirmation>(value);
                    if (saleconfirmation != null)
                    {
                        loadingDocument = new LoadingDocument
                        {
                            Id = value,
                            Customer = saleconfirmation.Customer,
                            Provider = saleconfirmation.Provider,
                            Transporter = saleconfirmation.Transporter,
                            DocumentDate = saleconfirmation.DocumentDate,
                            ProductDetails = saleconfirmation.ProductDetails,
                            ShippingDate = saleconfirmation.ShippingDate,
                            DeliveryDate = saleconfirmation.DeliveryDate,
                            TruckLicensePlate = saleconfirmation.TruckLicensePlate,
                            Rental = saleconfirmation.Rental,
                            DeliveryEx = saleconfirmation.DeliveryEx,
                            TermsOfPayment = saleconfirmation.TermsOfPayment,
                            InvoiceDiscount = saleconfirmation.InvoiceDiscount,
                            CustomerCommission = saleconfirmation.CustomerCommission,
                            ProviderCommission = saleconfirmation.ProviderCommission,
                            Notes = saleconfirmation.Notes,
                            Lot = saleconfirmation.Lot,
                            OrderCode = saleconfirmation.OrderCode
                        };
                        Status = "Documento numero " + value + " caricato correttamente";
                        session.Store(loadingDocument);
                        session.SaveChanges();
                    }
                    else
                    {
                        loadingDocument = new LoadingDocument{ Id = -1 };
                        Status = "Documento numero " + value + "non trovato";
                    }
                }
                else
                {
                    Status = "Documento numero " + value + " caricato correttamente";
                }
            }
            LoadingDocument = loadingDocument;
            CompanyControlViewModel.Company = LoadingDocument.Customer;
            ProviderControlViewModel.Company = LoadingDocument.Provider;
            TransporterControlViewModel.Company = LoadingDocument.Transporter;
            ProductDetails.Clear();
            foreach (var productDetail in loadingDocument.ProductDetails)
            {
                ProductDetails.Add(new ProductRowViewModel(productDetail));
            }
            UpdateTotals();
        }

        private Action SaveDocumentAction()
        {
            return SaveAllData;
        }

        private void SaveAllData()
        {
            LoadingDocument.Customer = CompanyControlViewModel.Company;
            LoadingDocument.Provider = ProviderControlViewModel.Company;
            LoadingDocument.Transporter = TransporterControlViewModel.Company;
            LoadingDocument.ProductDetails = new List<ProductDetails>();
            try
            {
                using (var session = _dataStorage.CreateSession())
                {
                    var savedProductIds = new List<int?>();
                    var savedCurrencies = new List<string>();
                    foreach (var productRowViewModel in ProductDetails)
                    {
                        UpdateProductDescriptionsAndCurrencies(productRowViewModel.ProductDetails, session, savedProductIds,
                            savedCurrencies);
                    }

                    UpdateTermsOfPayment(LoadingDocument.TermsOfPayment, session);

                    if (!string.IsNullOrWhiteSpace(CompanyControlViewModel.Company.CompanyName))
                        session.Store(CompanyControlViewModel.Company);
                    if (!string.IsNullOrWhiteSpace(ProviderControlViewModel.Company.CompanyName))
                        session.Store(ProviderControlViewModel.Company);
                    if (!string.IsNullOrWhiteSpace(TransporterControlViewModel.Company.CompanyName))
                        session.Store(TransporterControlViewModel.Company);
                    session.Store(LoadingDocument);
                    session.SaveChanges();
                }
                Id = LoadingDocument.Id;
                Status = "Salvato correttamente alle ore: " + DateTime.Now.ToShortTimeString();
            }
            catch (Exception exception)
            {
                Status = "Errore durante il salvataggio: " + exception.Message;
            }
        }

        private Action ReloadAction()
        {
            return () => LoadDocument(Id);
        }

        private static void UpdateTermsOfPayment(string termsOfPayment, IDocumentSession session)
        {
            if (string.IsNullOrWhiteSpace(termsOfPayment)) return;
            var top = session.Query<TermOfPayment>()
                .FirstOrDefault(tp => tp.Description.Equals(termsOfPayment, StringComparison.CurrentCultureIgnoreCase)) ??
                new TermOfPayment {Description = termsOfPayment};
            session.Store(top);
        }

        private void UpdateProductDescriptionsAndCurrencies(ProductDetails productDetails, IDocumentSession session, List<int?> savedProductIds, List<string> savedCurrencies)
        {
            LoadingDocument.ProductDetails.Add(productDetails);
            CheckIfProductDescriptionIsNew(productDetails, session, savedProductIds);
            CheckIfCurrencyIsNew(productDetails.Currency, session, savedCurrencies);
        }

        private static void CheckIfProductDescriptionIsNew(ProductDetails productDetails, IDocumentSession session,
            List<int?> savedProductIds)
        {
            if (!productDetails.ProductId.HasValue || !savedProductIds.Contains(productDetails.ProductId))
            {
                var productDescription =
                    session.Query<ProductDescription>()
                        .FirstOrDefault(
                            p => p.Description.Equals(productDetails.Description, StringComparison.CurrentCultureIgnoreCase));
                if (productDescription == null)
                {
                    productDescription = new ProductDescription
                    {
                        Description = productDetails.Description
                    };
                    session.Store(productDescription);
                }
                productDetails.ProductId = productDescription.Id;
                savedProductIds.Add(productDescription.Id);
            }
        }

        private static void CheckIfCurrencyIsNew(string currencyUsed, IDocumentSession session, List<string> savedCurrencies)
        {
            if (!string.IsNullOrWhiteSpace(currencyUsed) &&
                !savedCurrencies.Contains(currencyUsed.ToLowerInvariant()))
            {
                var currency =
                    session.Query<Currency>()
                        .FirstOrDefault(p => p.Name.Equals(currencyUsed, StringComparison.CurrentCultureIgnoreCase));
                if (currency == null)
                {
                    currency = new Currency
                    {
                        Name = currencyUsed
                    };
                    session.Store(currency);
                }
                savedCurrencies.Add(currency.Name.ToLowerInvariant());
            }
        }

        void ProductDetails_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (ProductRowViewModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (ProductRowViewModel item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateTotals();
        }

        private void UpdateTotals()
        {
            OnPropertyChanged("TotalPallets");
            OnPropertyChanged("TotalPackages");
            OnPropertyChanged("TotalGross");
            OnPropertyChanged("TotalNet");
            OnPropertyChanged("TotalAmount");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private ICommand saveAllCommand;

        private readonly IDataStorage _dataStorage;
        private readonly IWindowManager _windowManager;
        private string _status;
        private ICommand reloadCommand;
        private ICommand printDocument;
        private ICommand convertDocument;
    }
}
