using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Models;
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
                    var positions = session.Query<SaleConfirmation>().Customize(cr => cr.WaitForNonStaleResults()).Take(100).ToList();
                    return positions.Select<SaleConfirmation, int>(s => s.Id);
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