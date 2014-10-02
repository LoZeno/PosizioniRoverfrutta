using System.ComponentModel;
using System.Runtime.CompilerServices;
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
        }

        public CustomerControlViewModel CustomerControlViewModel { get; private set; }

        public CustomerControlViewModel ProviderControlViewModel { get; private set; }

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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly IDataStorage _dataStorage;
    }
}