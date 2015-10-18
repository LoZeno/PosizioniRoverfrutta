using System.Windows;
using CustomWPFControls;
using PosizioniRoverfrutta.Services;

namespace PosizioniRoverfrutta.CustomControls
{
    public class CurrenciesAutocompleteBoxWrapper : AutoCompleteBoxAsync
    {
        public CurrenciesAutocompleteBoxWrapper()
            : base()
        {
            this.DataProvider = new CurrenciesAutoCompleteBoxProvider(((App)Application.Current).DataStorage);
        }
    }
}