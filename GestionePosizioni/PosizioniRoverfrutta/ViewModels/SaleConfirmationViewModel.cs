using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Models;
using PosizioniRoverfrutta.Annotations;
using PosizioniRoverfrutta.Reports;
using QueryManager;
using Raven.Client;

namespace PosizioniRoverfrutta.ViewModels
{
    public class SaleConfirmationViewModel : INotifyPropertyChanged
    {
        public SaleConfirmationViewModel(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
            CompanyControlViewModel = new CompanyControlViewModel<Customer>(_dataStorage);
            ProviderControlViewModel = new CompanyControlViewModel<Customer>(_dataStorage);
            TransporterControlViewModel = new CompanyControlViewModel<Transporter>(_dataStorage);

            SaleConfirmation = new SaleConfirmation();
            ProductDetails = new ObservableCollection<ProductRowViewModel>();
            ProductDetails.CollectionChanged += ProductDetails_CollectionChanged;
        }

        public int Id
        {
            get { return SaleConfirmation.Id; }
            set
            {
                LoadDocument(value);

                OnPropertyChanged();
                OnPropertyChanged("SaleConfirmation");
                OnPropertyChanged("ShippingDate");
                OnPropertyChanged("DeliveryDate");
                OnPropertyChanged("TruckLicensePlate");
                OnPropertyChanged("Rental");
                OnPropertyChanged("DeliveryEx");
                OnPropertyChanged("TermsOfPayment");
                OnPropertyChanged("InvoiceDiscount");
                OnPropertyChanged("CustomerCommission");
                OnPropertyChanged("Notes");
                OnPropertyChanged("Lot");
                OnPropertyChanged("OrderCode");
            }
        }

        public DateTime? ShippingDate
        {
            get { return SaleConfirmation.ShippingDate; }
            set
            {
                SaleConfirmation.ShippingDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime? DeliveryDate
        {
            get { return SaleConfirmation.DeliveryDate; }
            set
            {
                SaleConfirmation.DeliveryDate = value;
                OnPropertyChanged();
            }
        }

        public string TruckLicensePlate
        {
            get { return SaleConfirmation.TruckLicensePlate; }
            set
            {
                SaleConfirmation.TruckLicensePlate = value;
                OnPropertyChanged();
            }
        }

        public decimal? Rental
        {
            get { return SaleConfirmation.Rental; }
            set
            {
                SaleConfirmation.Rental = value;
                OnPropertyChanged();
            }
        }

        public string DeliveryEx
        {
            get { return SaleConfirmation.DeliveryEx; }
            set
            {
                SaleConfirmation.DeliveryEx = value;
                OnPropertyChanged();
            }
        }

        public string TermsOfPayment
        {
            get { return SaleConfirmation.TermsOfPayment; }
            set
            {
                SaleConfirmation.TermsOfPayment = value;
                OnPropertyChanged();
            }
        }

        public decimal? InvoiceDiscount
        {
            get { return SaleConfirmation.InvoiceDiscount; }
            set
            {
                SaleConfirmation.InvoiceDiscount = value;
                OnPropertyChanged();
            }
        }

        public decimal? CustomerCommission
        {
            get { return SaleConfirmation.CustomerCommission; }
            set
            {
                SaleConfirmation.CustomerCommission = value;
                OnPropertyChanged();
            }
        }

        public string Notes
        {
            get { return SaleConfirmation.Notes; }
            set
            {
                SaleConfirmation.Notes = value;
                OnPropertyChanged();
            }

        }
        public string Lot
        {
            get { return SaleConfirmation.Lot; }
            set
            {
                SaleConfirmation.Lot = value;
                OnPropertyChanged();
            }

        }
        public string OrderCode
        {
            get { return SaleConfirmation.OrderCode; }
            set
            {
                SaleConfirmation.OrderCode = value;
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

        public CompanyControlViewModel<Customer> CompanyControlViewModel { get; private set; }

        public CompanyControlViewModel<Customer> ProviderControlViewModel { get; private set; }

        public CompanyControlViewModel<Transporter> TransporterControlViewModel { get; private set; }

        public ObservableCollection<ProductRowViewModel> ProductDetails { get; private set; }

        public ICommand Reload
        {
            get { return reloadCommand ?? (reloadCommand = new DelegateCommand(ReloadAction())); }
        }

        public SaleConfirmation SaleConfirmation { get; set; }

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

        private Action PrintDocument()
        {
            return delegate
            {
                var report = new SaleConfirmationReport(this.SaleConfirmation, @"D:\LoZeno\Documenti\ConfermaVendita-" + Id + ".pdf");
                report.CreatePdf();
            };
        }

        private void LoadDocument(int value)
        {
            SaleConfirmation saleConfirmation = null;
            using (var session = _dataStorage.CreateSession())
            {
                saleConfirmation = session.Load<SaleConfirmation>(value);
            }
            if (saleConfirmation == null)
            {
                saleConfirmation = new SaleConfirmation();
            }
            SaleConfirmation = saleConfirmation;
            CompanyControlViewModel.Company = SaleConfirmation.Customer;
            ProviderControlViewModel.Company = SaleConfirmation.Provider;
            TransporterControlViewModel.Company = SaleConfirmation.Transporter;
            ProductDetails.Clear();
            foreach (var productDetail in saleConfirmation.ProductDetails)
            {
                ProductDetails.Add(new ProductRowViewModel(productDetail));
            }
            UpdateTotals();
            Status = "Documento numero " + SaleConfirmation.Id + " caricato correttamente";
        }

        private Action SaveDocumentAction()
        {
            return delegate
            {
                SaleConfirmation.Customer = CompanyControlViewModel.Company;
                SaleConfirmation.Provider = ProviderControlViewModel.Company;
                SaleConfirmation.Transporter = TransporterControlViewModel.Company;
                SaleConfirmation.ProductDetails = new List<ProductDetails>();
                try
                {
                    using (var session = _dataStorage.CreateSession())
                    {
                        var savedProductIds = new List<int?>();
                        var savedCurrencies = new List<string>();
                        foreach (var productRowViewModel in ProductDetails)
                        {
                            UpdateProductDescriptionsAndCurrencies(productRowViewModel.ProductDetails, session, savedProductIds, savedCurrencies);
                        }

                        UpdateTermsOfPayment(SaleConfirmation.TermsOfPayment, session);

                        if (!string.IsNullOrWhiteSpace(CompanyControlViewModel.Company.CompanyName))
                            session.Store(CompanyControlViewModel.Company);
                        if (!string.IsNullOrWhiteSpace(ProviderControlViewModel.Company.CompanyName))
                            session.Store(ProviderControlViewModel.Company);
                        if (!string.IsNullOrWhiteSpace(TransporterControlViewModel.Company.CompanyName))
                            session.Store(TransporterControlViewModel.Company);
                        session.Store(SaleConfirmation);
                        session.SaveChanges();
                    }
                    Id = SaleConfirmation.Id;
                    Status = "Salvato correttamente alle ore: " + DateTime.Now.ToShortTimeString();
                }
                catch (Exception exception)
                {
                    Status = "Errore durante il salvataggio: " + exception.Message;
                }
            };
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
            SaleConfirmation.ProductDetails.Add(productDetails);
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
        private string _status;
        private ICommand reloadCommand;
        private ICommand printDocument;
    }
}