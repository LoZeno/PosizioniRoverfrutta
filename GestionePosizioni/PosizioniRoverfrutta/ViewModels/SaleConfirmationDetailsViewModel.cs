using System.ComponentModel;
using System.Runtime.CompilerServices;
using Models;
using PosizioniRoverfrutta.Annotations;

namespace PosizioniRoverfrutta.ViewModels
{
    public class SaleConfirmationDetailsViewModel : INotifyPropertyChanged
    {

        public SaleConfirmation SaleConfirmationDetails
        {
            get { return _saleConfirmationDetails; }
            set
            {
                _saleConfirmationDetails = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private SaleConfirmation _saleConfirmationDetails = new SaleConfirmation();
    }
}