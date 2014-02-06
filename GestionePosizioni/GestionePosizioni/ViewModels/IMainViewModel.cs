using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Models;

namespace GestionePosizioni.ViewModels
{
    public interface IMainViewModel : INotifyPropertyChanged
    {
        int? DocumentId { get; set; }
        Customer Customer { get; set; }
        Customer Provider { get; set; }
        ObservableCollection<ProductSold> Products { get; }
        int TotalPallets { get; }
        decimal TotalGrossWeight { get; }
        decimal TotalNetWeight { get; }
        int TotalPackages { get; }
        ICommand Save { get; }
    }
}