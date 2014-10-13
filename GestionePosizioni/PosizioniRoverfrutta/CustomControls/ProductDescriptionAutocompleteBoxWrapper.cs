using System.Windows;
using dragonz.actb.control;
using PosizioniRoverfrutta.Services;

namespace PosizioniRoverfrutta.CustomControls
{
    public class ProductDescriptionAutocompleteBoxWrapper : AutoCompleteComboBox
    {
        public ProductDescriptionAutocompleteBoxWrapper()
            :base()
        {
            AutoCompleteManager.DataProvider = new ProductNamesAutoCompleteBoxProvider(((App)Application.Current).DataStorage);
            AutoCompleteManager.Asynchronous = true;
            AutoCompleteManager.AutoAppend = true;
        }
    }
}
