using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Models;
using PosizioniRoverfrutta.Annotations;
using QueryManager.Repositories;

namespace PosizioniRoverfrutta.ViewModels
{
    internal class CustomerViewModel : INotifyPropertyChanged
    {

        public CustomerViewModel()
        {
            _repo = new CustomerRepository();
        }

        public Customer Customer
        {
            get { return _customer; }
            set
            {
                _customer = value;
                OnPropertyChanged();
            }
        }

        public ICommand Save
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new DelegateCommand(SaveModel());
                }
                return _saveCommand;
            }
        }

        private Action SaveModel()
        {
            return () => _repo.Add(_customer);
        }

        private Customer _customer;

        private bool _canSave = false;

        private ICommand _saveCommand;

        private CustomerRepository _repo;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}