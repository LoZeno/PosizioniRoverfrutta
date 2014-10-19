using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Models.DocumentTypes;
using PosizioniRoverfrutta.Annotations;
using QueryManager;

namespace PosizioniRoverfrutta.ViewModels
{
    public class ListPositionsViewModel : INotifyPropertyChanged
    {
        public ListPositionsViewModel(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }

        public IEnumerable<int> Positions 
        {
            get 
            {
                using (var session = _dataStorage.CreateSession())
                {
                    return session.Query<SaleConfirmation>().Customize(cr => cr.WaitForNonStaleResults()).ToList().OrderBy(sc => sc.Id).Select(s => s.Id);
                }
            }
        }

        private readonly IDataStorage _dataStorage;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}