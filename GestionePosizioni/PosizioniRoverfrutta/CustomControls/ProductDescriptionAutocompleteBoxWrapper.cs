using System.Windows;
using CustomWPFControls;
using PosizioniRoverfrutta.Services;

namespace PosizioniRoverfrutta.CustomControls
{
    public class ProductDescriptionAutocompleteBoxWrapper : AutoCompleteBoxAsync
    {
        public ProductDescriptionAutocompleteBoxWrapper()
            :base()
        {
            this.IsTextCompletionEnabled = true;
            DataProvider = new ProductNamesAutoCompleteBoxProvider(((App)Application.Current).DataStorage);
        }
    }
}
