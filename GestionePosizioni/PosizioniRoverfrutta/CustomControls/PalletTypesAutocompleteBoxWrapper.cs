using CustomWPFControls;
using PosizioniRoverfrutta.Services;

namespace PosizioniRoverfrutta.CustomControls
{
    public class PalletTypesAutocompleteBoxWrapper : AutoCompleteBoxAsync
    {
        public PalletTypesAutocompleteBoxWrapper()
        {
            this.DataProvider = new PalletTypesComboBoxProvider();
            this.GotFocus += PalletTypesAutocompleteBoxWrapper_GotFocus;
        }

        private void PalletTypesAutocompleteBoxWrapper_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Text = " ";
            }
            this.PopulateComplete();
        }
    }
}
