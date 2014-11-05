using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Models.Companies;
using Models.DocumentTypes;
using Models.Entities;
using PosizioniRoverfrutta.Annotations;
using PosizioniRoverfrutta.Reports;
using PosizioniRoverfrutta.Windows;
using QueryManager;
using Raven.Client;

namespace PosizioniRoverfrutta.ViewModels
{
    public class PriceConfirmationViewModel : INotifyPropertyChanged
    {
        public PriceConfirmationViewModel(IDataStorage dataStorage, IWindowManager windowManager)
        {
            _dataStorage = dataStorage;
            _windowManager = windowManager;
            CompanyControlViewModel = new CompanyControlViewModel<Customer>(_dataStorage);
            ProviderControlViewModel = new CompanyControlViewModel<Customer>(_dataStorage);
            TransporterControlViewModel = new CompanyControlViewModel<Transporter>(_dataStorage);

            PriceConfirmation = new PriceConfirmation();
            ProductDetails = new ObservableCollection<ProductRowViewModel>();
            ProductDetails.CollectionChanged += ProductDetails_CollectionChanged;
        }

        public int Id
        {
            get { return PriceConfirmation.ProgressiveNumber; }
            set
            {
                LoadDocument(value);

                OnPropertyChanged();
                OnPropertyChanged("PriceConfirmation");
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
                OnPropertyChanged("Vat");
            }
        }

        public DateTime? DocumentDate
        {
            get { return PriceConfirmation.DocumentDate; }
            set
            {
                PriceConfirmation.DocumentDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime? ShippingDate
        {
            get { return PriceConfirmation.ShippingDate; }
            set
            {
                PriceConfirmation.ShippingDate = value;
                OnPropertyChanged();
            }
        }

        public string TransportDocument
        {
            get { return PriceConfirmation.TransportDocument; }
            set
            {
                PriceConfirmation.TransportDocument = value;
                OnPropertyChanged();
            }
        }

        public string TruckLicensePlate
        {
            get { return PriceConfirmation.TruckLicensePlate; }
            set
            {
                PriceConfirmation.TruckLicensePlate = value;
                OnPropertyChanged();
            }
        }

        public decimal? Rental
        {
            get { return PriceConfirmation.Rental; }
            set
            {
                PriceConfirmation.Rental = value;
                OnPropertyChanged();
            }
        }

        public string DeliveryEx
        {
            get { return PriceConfirmation.DeliveryEx; }
            set
            {
                PriceConfirmation.DeliveryEx = value;
                OnPropertyChanged();
            }
        }

        public string TermsOfPayment
        {
            get { return PriceConfirmation.TermsOfPayment; }
            set
            {
                PriceConfirmation.TermsOfPayment = value;
                OnPropertyChanged();
            }
        }

        public decimal? InvoiceDiscount
        {
            get { return PriceConfirmation.InvoiceDiscount; }
            set
            {
                PriceConfirmation.InvoiceDiscount = value ?? 0;
                OnPropertyChanged();
                UpdateTotals();
            }
        }

        public decimal? CustomerCommission
        {
            get { return PriceConfirmation.CustomerCommission; }
            set
            {
                PriceConfirmation.CustomerCommission = value;
                OnPropertyChanged();
                UpdateTotals();
            }
        }


        public decimal? ProviderCommission
        {
            get { return PriceConfirmation.ProviderCommission; }
            set
            {
                PriceConfirmation.ProviderCommission = value;
                OnPropertyChanged();
                UpdateTotals();
            }
        }

        public string Notes
        {
            get { return PriceConfirmation.Notes; }
            set
            {
                PriceConfirmation.Notes = value;
                OnPropertyChanged();
            }

        }
        public string Lot
        {
            get { return PriceConfirmation.Lot; }
            set
            {
                PriceConfirmation.Lot = value;
                OnPropertyChanged();
            }

        }
        public string OrderCode
        {
            get { return PriceConfirmation.OrderCode; }
            set
            {
                PriceConfirmation.OrderCode = value;
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
            get { return Math.Round(ProductDetails.Sum(p => p.GrossWeight), 2); }
        }

        public decimal TotalNet
        {
            get { return Math.Round(ProductDetails.Sum(p => p.NetWeight), 2); }
        }

        public decimal TotalAmount
        {
            get { return PriceConfirmation.TotalAmount; }
            set
            {
                PriceConfirmation.TotalAmount = value;
                OnPropertyChanged();
            }
        }

        public decimal CalculatedDiscount
        {
            get { return PriceConfirmation.CalculatedDiscount; }
            set
            {
                PriceConfirmation.CalculatedDiscount = value;
                OnPropertyChanged();
            }
        }

        public decimal TaxableAmount
        {
            get { return PriceConfirmation.TaxableAmount; }
            set
            {
                PriceConfirmation.TaxableAmount = value;
                OnPropertyChanged();
            }
        }

        public decimal Vat
        {
            get { return PriceConfirmation.Vat; }
            set
            {
                PriceConfirmation.Vat = value;
                OnPropertyChanged();
                UpdateTotals();
            }
        }

        public decimal CalculatedVat
        {
            get { return PriceConfirmation.CalculatedVat; }
            set
            {
                PriceConfirmation.CalculatedVat = value;
                OnPropertyChanged();
            }
        }

        public decimal FinalTotal
        {
            get { return PriceConfirmation.FinalTotal; }
            set
            {
                PriceConfirmation.FinalTotal = value;
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

        public PriceConfirmation PriceConfirmation { get; set; }

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

        public ICommand Email
        {
            get { return emailDocument ?? (emailDocument = new DelegateCommand(SendEmail())); }
        }

        private Action SendEmail()
        {
            return delegate
            {
                SaveAllData();
                var path = SavePdf();
                _windowManager.InstantiateWindow(path, WindowTypes.InviaEmail);
            };
        }

        private Action PrintDocument()
        {
            return delegate
            {
                SaveAllData();
                SavePdf();
            };
        }

        private string SavePdf()
        {
            var path = _windowManager.OpenSaveToPdfDialog(string.Format("ConfermaPrezzi-{0}", Id));
            if (string.IsNullOrWhiteSpace(path))
            {
                Status = "Creazione del PDF annullata";
                return string.Empty;
            }
            var report = new PriceConfirmationReport(PriceConfirmation, path);
            report.CreatePdf();
            Status = string.Format("PDF del Documento n° {0} creato correttamente", Id);
            return path;
        }

        private void LoadDocument(int value)
        {
            PriceConfirmation priceConfirmation;
            using (var session = _dataStorage.CreateSession())
            {
                priceConfirmation = session.Load<PriceConfirmation>("PriceConfirmations/" + value);
            
                if (priceConfirmation == null)
                {
                    var loadingDocument = session.Load<LoadingDocument>("LoadingDocuments/"+value);
                    if (loadingDocument != null)
                    {
                        priceConfirmation = new PriceConfirmation
                        {
                            Id = "PriceConfirmations/"+value,
                            Customer = loadingDocument.Customer,
                            Provider = loadingDocument.Provider,
                            Transporter = loadingDocument.Transporter,
                            DocumentDate = loadingDocument.DocumentDate,
                            ProductDetails = loadingDocument.ProductDetails,
                            ShippingDate = loadingDocument.ShippingDate,
                            DeliveryDate = loadingDocument.DeliveryDate,
                            TruckLicensePlate = loadingDocument.TruckLicensePlate,
                            Rental = loadingDocument.Rental,
                            DeliveryEx = loadingDocument.DeliveryEx,
                            TermsOfPayment = loadingDocument.TermsOfPayment,
                            InvoiceDiscount = loadingDocument.InvoiceDiscount,
                            CustomerCommission = loadingDocument.CustomerCommission,
                            ProviderCommission = loadingDocument.ProviderCommission,
                            Notes = loadingDocument.Notes,
                            Lot = loadingDocument.Lot,
                            OrderCode = loadingDocument.OrderCode
                        };
                        var initialVat = session.Load<DefaultValues>(1).Vat;
                        priceConfirmation.Vat = initialVat;
                        Status = "Documento numero " + value + " caricato correttamente";
                        session.Store(priceConfirmation);
                        session.SaveChanges();
                    }
                    else
                    {
                        priceConfirmation = new PriceConfirmation();
                        Status = "Documento numero " + value + "non trovato";
                    }
                }
                else
                {
                    Status = "Documento numero " + value + " caricato correttamente";
                }
            }
            PriceConfirmation = priceConfirmation;
            CompanyControlViewModel.Company = PriceConfirmation.Customer;
            ProviderControlViewModel.Company = PriceConfirmation.Provider;
            TransporterControlViewModel.Company = PriceConfirmation.Transporter;
            ProductDetails.Clear();
            foreach (var productDetail in priceConfirmation.ProductDetails)
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
            PriceConfirmation.Customer = CompanyControlViewModel.Company;
            PriceConfirmation.Provider = ProviderControlViewModel.Company;
            PriceConfirmation.Transporter = TransporterControlViewModel.Company;
            PriceConfirmation.ProductDetails = new List<ProductDetails>();
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

                    UpdateTermsOfPayment(PriceConfirmation.TermsOfPayment, session);

                    UpdateDefaultVat(PriceConfirmation.Vat, session);

                    if (!string.IsNullOrWhiteSpace(CompanyControlViewModel.Company.CompanyName))
                        session.Store(CompanyControlViewModel.Company);
                    if (!string.IsNullOrWhiteSpace(ProviderControlViewModel.Company.CompanyName))
                        session.Store(ProviderControlViewModel.Company);
                    if (!string.IsNullOrWhiteSpace(TransporterControlViewModel.Company.CompanyName))
                        session.Store(TransporterControlViewModel.Company);
                    session.Store(PriceConfirmation);
                    session.SaveChanges();
                }
                Id = PriceConfirmation.ProgressiveNumber;
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

        private void UpdateDefaultVat(decimal vat, IDocumentSession session)
        {
            var defaults = session.Load<DefaultValues>(1);
            defaults.Vat = vat;
            session.Store(defaults);
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
            PriceConfirmation.ProductDetails.Add(productDetails);
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
            TotalAmount = Math.Round(PriceConfirmation.ProductDetails.Sum(p => p.TotalPrice), 2);
            if (!PriceConfirmation.InvoiceDiscount.HasValue)
            {
                PriceConfirmation.InvoiceDiscount = 0;
            }
            CalculatedDiscount = Math.Round(((PriceConfirmation.TotalAmount*PriceConfirmation.InvoiceDiscount.Value)/100), 2);
            TaxableAmount = PriceConfirmation.TotalAmount - PriceConfirmation.CalculatedDiscount;
            var calculatedVat = Math.Round(((PriceConfirmation.TaxableAmount*PriceConfirmation.Vat)/100), 2);
            CalculatedVat = Math.Round(calculatedVat, 2);
            FinalTotal = PriceConfirmation.TaxableAmount + PriceConfirmation.CalculatedVat;
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
        private ICommand emailDocument;
    }
}