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
            CustomerControlViewModel = new CustomerControlViewModel();
            ProviderControlViewModel = new CustomerControlViewModel();
            //inizializzare viewmodel del trasportatore
            //viewmodel dei prodotti
            //model del documento vero e proprio
            SaleConfirmation = new SaleConfirmation();
            ProductDetails = new ObservableCollection<ProductRowViewModel>();
        }

        public CustomerControlViewModel CustomerControlViewModel { get; private set; }

        public CustomerControlViewModel ProviderControlViewModel { get; private set; }

        public ObservableCollection<ProductRowViewModel> ProductDetails { get; private set; }

        public SaleConfirmation SaleConfirmation { get; set; }

        public int Id
        {
            get { return SaleConfirmation.Id; }
            set
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
                CustomerControlViewModel.Customer = SaleConfirmation.Customer;
                ProviderControlViewModel.Customer = SaleConfirmation.Provider;
                OnPropertyChanged("SaleConfirmation");
            }
        }

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
                SaleConfirmation.Customer = CustomerControlViewModel.Customer;
                using (var session = _dataStorage.CreateSession())
                {
                    session.Store(CustomerControlViewModel.Customer);
                    session.Store(SaleConfirmation);
                    session.SaveChanges();
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
    }
}