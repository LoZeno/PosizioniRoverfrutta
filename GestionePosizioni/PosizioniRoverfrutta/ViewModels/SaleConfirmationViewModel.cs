using System.ComponentModel;
using System.Runtime.CompilerServices;
using PosizioniRoverfrutta.Annotations;

namespace PosizioniRoverfrutta.ViewModels
{
    public class SaleConfirmationViewModel : INotifyPropertyChanged
    {
        public SaleConfirmationViewModel()
        {
            CustomerControlViewModel = new CustomerControlViewModel();
            ProviderControlViewModel = new CustomerControlViewModel();
            //inizializzare viewmodel del trasportatore
            //viewmodel dei prodotti
            //viewmodel del documento vero e proprio
        }

        public CustomerControlViewModel CustomerControlViewModel { get; private set; }

        public CustomerControlViewModel ProviderControlViewModel { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}