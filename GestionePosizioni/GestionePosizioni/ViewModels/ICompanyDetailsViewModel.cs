using System.Collections.ObjectModel;
using System.ComponentModel;
using Models;

namespace GestionePosizioni.ViewModels
{
    public interface ICompanyDetailsViewModel : INotifyPropertyChanged
    {
        string Id { get; set; }
        string CompanyName { get; set; }
        string Address { get; set; }
        string City { get; set; }
        string StateOrProvince { get; set; }
        string PostCode { get; set; }
        string Country { get; set; }
        string VatCode { get; set; }

        Models.CompanyBase Company { get; set; }
    }
}