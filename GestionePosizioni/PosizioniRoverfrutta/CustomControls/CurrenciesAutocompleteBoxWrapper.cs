using System.Windows;
using dragonz.actb.control;
using PosizioniRoverfrutta.Services;

namespace PosizioniRoverfrutta.CustomControls
{
    public class CurrenciesAutocompleteBoxWrapper : AutoCompleteComboBox
    {
        public CurrenciesAutocompleteBoxWrapper()
            : base()
        {
            AutoCompleteManager.DataProvider = new CurrenciesAutoCompleteBoxProvider(((App)Application.Current).DataStorage);
            AutoCompleteManager.Asynchronous = true;
            AutoCompleteManager.AutoAppend = true;
        }
    }
}