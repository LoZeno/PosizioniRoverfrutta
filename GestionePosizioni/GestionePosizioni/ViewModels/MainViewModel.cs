using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;

namespace GestionePosizioni.ViewModels
{
    public class MainViewModel :INotifyPropertyChanged
    {

        public MainViewModel()
        {
            _customersClickCommand = new DelegateCommand(OnCustomersClick);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private ICommand _customersClickCommand;
        public ICommand CustomersClickCommand
        {
            get { return _customersClickCommand; }
        }

        private void OnCustomersClick()
        {

        }
    }
}
