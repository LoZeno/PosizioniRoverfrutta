using System.ComponentModel;
using System.Runtime.CompilerServices;
using PosizioniRoverfrutta.Annotations;

namespace PosizioniRoverfrutta.ViewModels.Statistics
{
    public class ProductsStatisticsViewModel : INotifyPropertyChanged
    {


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}