using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MailWrapper;
using Microsoft.Practices.Prism.Commands;
using Models.Companies;
using Models.DocumentTypes;
using Models.Entities;
using PosizioniRoverfrutta.Annotations;
using PosizioniRoverfrutta.Reports;
using PosizioniRoverfrutta.Windows;
using QueryManager;
using Raven.Client;
using ReportManager;

namespace PosizioniRoverfrutta.ViewModels
{
    public class SaleConfirmationViewModel : INotifyPropertyChanged
    {
        public SaleConfirmationViewModel(IDataStorage dataStorage, IWindowManager windowManager)
        {
            _dataStorage = dataStorage;
            _windowManager = windowManager;
            CompanyControlViewModel = new CompanyControlViewModel<Customer>(_dataStorage);
            ProviderControlViewModel = new CompanyControlViewModel<Customer>(_dataStorage);
            TransporterControlViewModel = new CompanyControlViewModel<Transporter>(_dataStorage);

            SaleConfirmation = new SaleConfirmation();
            ProductDetails = new ObservableCollection<ProductRowViewModel>();
            ProductDetails.CollectionChanged += ProductDetails_CollectionChanged;
        }

        public int Id
        {
            get { return SaleConfirmation.ProgressiveNumber; }
            set
            {
                LoadDocument(value);

                OnPropertyChanged();
                OnPropertyChanged("SaleConfirmation");
                OnPropertyChanged("DocumentDate");
                OnPropertyChanged("ShippingDate");
                OnPropertyChanged("DeliveryDate");
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
            }
        }

        public DateTime? DocumentDate
        {
            get { return SaleConfirmation.DocumentDate; }
            set
            {
                SaleConfirmation.DocumentDate = value;
                OnPropertyChanged();
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


        public decimal? ProviderCommission
        {
            get { return SaleConfirmation.ProviderCommission; }
            set
            {
                SaleConfirmation.ProviderCommission = value;
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

        public decimal TotalPallets
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
            get { return Math.Round(ProductDetails.Sum(p => p.TotalPrice), 2); }
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
            get { return printDocument ?? (printDocument = new DelegateCommand(PrintDocument(true, true))); }
        }

        public ICommand PrintForProvider
        {
            get { return printDocumentForProvider ?? (printDocumentForProvider = new DelegateCommand(PrintDocument(true, false))); }
        }

        public ICommand PrintForCustomer
        {
            get { return printDocumentForCustomer ?? (printDocumentForCustomer = new DelegateCommand(PrintDocument(false, true))); }
        }

        public ICommand Convert
        {
            get { return convertDocument ?? (convertDocument = new DelegateCommand(ConvertDocument())); }
        }

        public ICommand Email
        {
            get { return emailDocument ?? (emailDocument = new DelegateCommand(SendEmail(true, true))); }
        }

        public ICommand EmailToProvider
        {
            get
            {
                return emailDocumentToProvider ?? (emailDocumentToProvider = new DelegateCommand(SendEmail(true, false)));
            }
        }

        public ICommand EmailToCustomer
        {
            get
            {
                return emailDocumentToCustomer ?? (emailDocumentToCustomer = new DelegateCommand(SendEmail(false, true)));
            }
        }

        public ICommand EmailToTransporter
        {
            get
            {
                return emailDocumentToTransporter ?? (emailDocumentToTransporter = new DelegateCommand(SendEmailNoAttachment()));
            }
        }

        private Action SendEmailNoAttachment()
        {
            return delegate
            {
                SaveAllData();
                var path = Path.Combine(_tempEmailAttachmentFolder, string.Format("EmailTrasportatore.{0}.html", SaleConfirmation.ProgressiveNumber));
                (new FileInfo(path)).Directory.Create();
                var emailText = new SaleConfirmationEmail(SaleConfirmation, path);
                emailText.GenerateEmail();
                MAPI email = new MAPI();
                email.AddAttachment(path);
                email.SendMailPopup(string.Format("Invio Conferma di Vendita n° {0}", SaleConfirmation.ProgressiveNumber),null);
            };
        }

        private Action SendEmail(bool printForProvider, bool printForCustomer)
        {
            return delegate
            {
                SaveAllData();
                var path = Path.Combine(_tempEmailAttachmentFolder, string.Format("{0}.{1}.pdf",FormatFileName(printForProvider, printForCustomer), SaleConfirmation.ProgressiveNumber));
                (new FileInfo(path)).Directory.Create();
                var report = new SaleConfirmationReport(SaleConfirmation, path, printForProvider, printForCustomer);
                report.CreatePdf();
                MAPI email = new MAPI();
                email.AddAttachment(path);
                email.SendMailPopup(string.Format("Invio Conferma di Vendita n° {0}", SaleConfirmation.ProgressiveNumber), string.Format("In allegato la conferma di vendita n° {0}", SaleConfirmation.ProgressiveNumber) );
            };
        }

        private Action ConvertDocument()
        {
            return delegate
            {
                SaveAllData();
                _windowManager.InstantiateWindow(Id.ToString(), WindowTypes.DistintaCarico);
            };
        }

        private Action PrintDocument(bool printForProvider, bool printForCustomer)
        {
            return delegate
            {
                SaveAllData();
                SavePdf(printForProvider, printForCustomer);
            };
        }

        private void SavePdf(bool printForProvider, bool printForCustomer)
        {
            try
            {
                var documentName = FormatFileName(printForProvider, printForCustomer);
                var path = _windowManager.OpenSaveToPdfDialog(string.Format("{0}-{1}", documentName, Id));
                if (string.IsNullOrWhiteSpace(path))
                {
                    Status = "Creazione del PDF annullata";
                    return;
                }
                var report = new SaleConfirmationReport(SaleConfirmation, path, printForProvider, printForCustomer);
                report.CreatePdf();
                Status = string.Format("PDF del Documento n° {0} creato correttamente", Id);
            }
            catch (Exception ex)
            {
                Status = string.Format("Errore durante la creazione del PDF: {0}", ex.Message);
            }
        }

        private static string FormatFileName(bool printForProvider, bool printForCustomer)
        {
            string documentName = "ConfermaVendita";
            if (printForProvider)
            {
                documentName = documentName + "-fornitore";
            }
            if (printForCustomer)
            {
                documentName = documentName + "-cliente";
            }
            return documentName;
        }

        private void LoadDocument(int value)
        {
            SaleConfirmation saleConfirmation = null;
            using (var session = _dataStorage.CreateSession())
            {
                saleConfirmation = session.Load<SaleConfirmation>("SaleConfirmations/"+value);
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
            Status = "Documento numero " + SaleConfirmation.ProgressiveNumber + " caricato correttamente";
        }

        private Action SaveDocumentAction()
        {
            return SaveAllData;
        }

        private void SaveAllData()
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
                        UpdateProductDescriptionsAndCurrencies(productRowViewModel.ProductDetails, session, savedProductIds,
                            savedCurrencies);
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
                Id = SaleConfirmation.ProgressiveNumber;
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
        private readonly IWindowManager _windowManager;
        private string _status;
        private ICommand reloadCommand;
        private ICommand printDocument;
        private ICommand printDocumentForCustomer;
        private ICommand printDocumentForProvider;
        private ICommand convertDocument;
        private ICommand emailDocument;
        private ICommand emailDocumentToCustomer;
        private ICommand emailDocumentToProvider;
        private ICommand emailDocumentToTransporter;
        private readonly string _tempEmailAttachmentFolder = Path.Combine(Path.GetTempPath(), "RoverfruttaAttachment");
    }
}