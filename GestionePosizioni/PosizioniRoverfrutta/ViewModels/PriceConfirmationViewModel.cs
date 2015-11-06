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
using PosizioniRoverfrutta.Services;
using PosizioniRoverfrutta.Windows;
using QueryManager;
using Raven.Client;
using Raven.Client.Linq;

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

            CompanyControlViewModel.PropertyChanged += SubViewModel_PropertyChanged;
            ProviderControlViewModel.PropertyChanged += SubViewModel_PropertyChanged;
            TransporterControlViewModel.PropertyChanged += SubViewModel_PropertyChanged;

            PriceConfirmation = new PriceConfirmation();
            ProductDetails = new ObservableCollection<ProductRowViewModel>();
            ProductDetails.CollectionChanged += ProductDetails_CollectionChanged;
        }

        public int Id
        {
            get { return PriceConfirmation.ProgressiveNumber; }
            set
            {
                bool canSave, canUseActions;
                LoadDocument(value, out canSave, out canUseActions);

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
                OnPropertyChanged("Vat");

                SaveButtonEnabled = canSave;
                ActionButtonsEnabled = canUseActions;
                ReloadButtonEnabled = false;
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

        public bool SaveButtonEnabled
        {
            get { return _saveButtonEnabled; }
            private set
            {
                _saveButtonEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool ActionButtonsEnabled
        {
            get { return _actionButtonsEnabled; }
            private set
            {
                _actionButtonsEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool ReloadButtonEnabled
        {
            get { return _reloadButtonEnabled; }
            private set
            {
                _reloadButtonEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool ShowVatArea
        {
            get { return (!CompanyControlViewModel.DoNotApplyVat && !ProviderControlViewModel.DoNotApplyVat); }
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

        private Action SendEmail(bool printForProvider, bool printForCustomer)
        {
            return delegate
            {
                SaveAllData();
                var path = Path.Combine(_tempEmailAttachmentFolder, string.Format("{0}.{1}.pdf", FormatFileName(printForProvider, printForCustomer), PriceConfirmation.ProgressiveNumber));
                (new FileInfo(path)).Directory.Create();
                var report = new PriceConfirmationReport(PriceConfirmation, path, printForProvider, printForCustomer);
                report.CreatePdf();
                MAPI email = new MAPI();
                if (printForProvider && !string.IsNullOrWhiteSpace(PriceConfirmation.Provider.EmailAddress))
                    email.AddRecipientTo(PriceConfirmation.Provider.EmailAddress);
                if (printForCustomer && !string.IsNullOrWhiteSpace(PriceConfirmation.Customer.EmailAddress))
                    email.AddRecipientTo(PriceConfirmation.Customer.EmailAddress);
                email.AddAttachment(path);
                email.SendMailPopup(string.Format("Invio Conferma Prezzi n° {0}", PriceConfirmation.ProgressiveNumber), string.Format("In allegato la conferma prezzi n° {0}", PriceConfirmation.ProgressiveNumber));
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
                var report = new PriceConfirmationReport(PriceConfirmation, path, printForProvider, printForCustomer);
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
            string documentName = "ConfermaPrezzi";
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

        private void LoadDocument(int value, out bool canSave, out bool canUseActions)
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
                            DocumentDate = DateTime.Today,
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
                            OrderCode = loadingDocument.OrderCode,
                            TransportDocument = loadingDocument.TransportDocument
                        };
                        var initialVat = session.Load<DefaultValues>(1).Vat;
                        priceConfirmation.Vat = initialVat;
                        Status = "Documento numero " + value + " caricato correttamente";
                        canSave = true;
                        canUseActions = false;
                        _windowManager.PopupMessage(string.Format("Conferma prezzi numero {0} creata, premere 'Salva' dopo aver apportato le modifiche necessarie", value), "Nuova Conferma Prezzi creata");
                    }
                    else
                    {
                        priceConfirmation = new PriceConfirmation();
                        canSave = false;
                        canUseActions = false;
                        Status = "Documento numero " + value + "non trovato";
                    }
                }
                else
                {
                    canSave = false;
                    canUseActions = true;
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
                    var savedPalletTypesIds = new List<string>();
                    foreach (var productRowViewModel in ProductDetails)
                    {
                        UpdateProductDescriptionsCurrenciesAndPalletTypes(productRowViewModel.ProductDetails, session, savedProductIds,
                            savedCurrencies, savedPalletTypesIds);
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
            return () =>
            {
                bool canSave, canUseActions;
                LoadDocument(Id, out canSave, out canUseActions);
                SaveButtonEnabled = canSave;
                ActionButtonsEnabled = canUseActions;
                ReloadButtonEnabled = false;
            };
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

        private void UpdateProductDescriptionsCurrenciesAndPalletTypes(ProductDetails productDetails, IDocumentSession session, List<int?> savedProductIds, List<string> savedCurrencies, List<string> savedPalletTypesIds)
        {
            PriceConfirmation.ProductDetails.Add(productDetails);
            CheckIfProductDescriptionIsNew(productDetails, session, savedProductIds);
            CheckIfCurrencyIsNew(productDetails.Currency, session, savedCurrencies);
            CheckIfPalletTypeIsNew(productDetails.PalletType, session, savedPalletTypesIds);
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

        private static void CheckIfPalletTypeIsNew(string palletTypeUsed, IDocumentSession session, List<string> savedPalletTypesIds)
        {
            if (!string.IsNullOrWhiteSpace(palletTypeUsed) &&
                !savedPalletTypesIds.Contains(palletTypeUsed.ToLowerInvariant()))
            {
                var palletType =
                    session.Query<PalletType>()
                    .FirstOrDefault(p => p.Name.Equals(palletTypeUsed, StringComparison.CurrentCultureIgnoreCase));
                if (palletType == null)
                {
                    palletType = new PalletType
                    {
                        Name = palletTypeUsed.Trim()
                    };
                    session.Store(palletType);
                }
                savedPalletTypesIds.Add(palletType.Name.ToLowerInvariant());
            }
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
            CalculatedVat = 0;
            if (!CompanyControlViewModel.DoNotApplyVat && !ProviderControlViewModel.DoNotApplyVat)
            {
                decimal calculatedVat = ((PriceConfirmation.TaxableAmount * PriceConfirmation.Vat) / 100);
                CalculatedVat = calculatedVat.RoundUp(2);
            }
            FinalTotal = PriceConfirmation.TaxableAmount + PriceConfirmation.CalculatedVat;
        }

        void ProductDetails_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (ProductRowViewModel item in e.NewItems)
                    item.PropertyChanged += observableCollectionItem_PropertyChanged;

            if (e.OldItems != null)
                foreach (ProductRowViewModel item in e.OldItems)
                    item.PropertyChanged -= observableCollectionItem_PropertyChanged;
            SaveButtonEnabled = true;
            ActionButtonsEnabled = false;
            ReloadButtonEnabled = true;
        }

        void observableCollectionItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateTotals();
        }

        void SubViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DoNotApplyVat")
            {
                UpdateTotals();
                OnPropertyChanged("ShowVatArea");
            }
            SaveButtonEnabled = true;
            ActionButtonsEnabled = false;
            ReloadButtonEnabled = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            if (!propertyName.In("SaveButtonEnabled", "ActionButtonsEnabled", "ReloadButtonEnabled", "Status"))
            {
                if (!SaveButtonEnabled)
                {
                    SaveButtonEnabled = true;
                }
                if (ActionButtonsEnabled)
                {
                    ActionButtonsEnabled = false;
                }
                if (!ReloadButtonEnabled)
                {
                    ReloadButtonEnabled = true;
                }
            }
        }

        private ICommand saveAllCommand;

        private readonly IDataStorage _dataStorage;
        private readonly IWindowManager _windowManager;
        private string _status;

        private bool _saveButtonEnabled = false;
        private bool _actionButtonsEnabled = false;
        private bool _reloadButtonEnabled = false;

        private ICommand reloadCommand;
        private ICommand printDocument;
        private ICommand printDocumentForCustomer;
        private ICommand printDocumentForProvider;
        private ICommand emailDocument;
        private ICommand emailDocumentToCustomer;
        private ICommand emailDocumentToProvider;
        private readonly string _tempEmailAttachmentFolder = Path.Combine(Path.GetTempPath(), "RoverfruttaAttachment");
    }
}