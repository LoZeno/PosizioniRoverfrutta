using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Models;
using PosizioniRoverfrutta.Annotations;
using QueryManager;

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
            //viewmodel dei prodotti
            //model del documento vero e proprio
            SaleConfirmation = new SaleConfirmation();
            ProductDetails = new ObservableCollection<ProductRowViewModel>();
        }

        public int Id
        {
            get { return SaleConfirmation.Id; }
            set
            {
                LoadDocument(value);

                OnPropertyChanged();
                OnPropertyChanged("SaleConfirmation");
            }
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
            Status = "Documento numero " + SaleConfirmation.Id + " caricato correttamente";
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

        public SaleConfirmation SaleConfirmation { get; set; }

        public ICommand SaveAll
        {
            get
            {
                return saveAllCommand ?? (saveAllCommand = new DelegateCommand(SaveDocumentAction()));
            }
        }

        private Action SaveDocumentAction()
        {
            return delegate
            {
                SaleConfirmation.ProductDetails = new List<ProductDetails>();
                foreach (var productRowViewModel in ProductDetails)
                {
                    SaleConfirmation.ProductDetails.Add(productRowViewModel.ProductDetails);
                }
                SaleConfirmation.Customer = CompanyControlViewModel.Company;
                SaleConfirmation.Provider = ProviderControlViewModel.Company;
                SaleConfirmation.Transporter = TransporterControlViewModel.Company;
                try
                {
                    using (var session = _dataStorage.CreateSession())
                    {
                        session.Store(CompanyControlViewModel.Company);
                        session.Store(ProviderControlViewModel.Company);
                        session.Store(TransporterControlViewModel.Company);
                        session.Store(SaleConfirmation);
                        session.SaveChanges();
                    }
                    OnPropertyChanged("Id");
                    Status = "Salvato correttamente alle ore: " + DateTime.Now.ToShortTimeString();
                }
                catch (Exception exception)
                {
                    Status = "Errore durante il salvataggio: " + exception.Message;
                }
            };
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
    }
}