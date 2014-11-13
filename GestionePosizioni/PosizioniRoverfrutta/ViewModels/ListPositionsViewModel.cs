using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Models.Entities;
using PosizioniRoverfrutta.Annotations;
using QueryManager;

namespace PosizioniRoverfrutta.ViewModels
{
    public class ListPositionsViewModel : INotifyPropertyChanged
    {
        public ListPositionsViewModel(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
            PositionsList = new ObservableCollection<PositionsListRow>();
        }

        public bool HasFocus
        {
            get { return _hasFocus; }
            set
            {
                _hasFocus = value;
                RefreshData();
                OnPropertyChanged();
            }
        }

        public string CompanyName
        {
            get { return _companyName; }
            set
            {
                _companyName = value;
                LoadCompanyId();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PositionsListRow> PositionsList { get; set; }

        private void LoadCompanyId()
        {
            //find Company Id
            if (_companyId.HasValue)
            {
                RefreshData();
            }
        }

        private void RefreshData()
        {
            throw new System.NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _hasFocus;
        private readonly IDataStorage _dataStorage;
        private string _companyName;
        private int? _companyId;
    }
}