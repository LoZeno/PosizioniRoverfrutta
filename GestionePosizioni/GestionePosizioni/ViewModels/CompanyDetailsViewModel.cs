using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using QueryManager.Repositories;

namespace GestionePosizioni.ViewModels
{
    public class CompanyDetailsViewModel : INotifyPropertyChanged
    {
        private CompanyBase _company;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public CompanyDetailsViewModel(CompanyBase company)
        {
            _company = company;
        }

        internal CompanyDetailsViewModel()
        {
            
        }
    }
}
