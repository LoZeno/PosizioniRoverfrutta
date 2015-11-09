using PosizioniRoverfrutta.Services;
using System.Windows.Controls;

namespace PosizioniRoverfrutta.CustomControls
{
    public class PalletTypesComboBoxWrapper : ComboBox
    {        
        public PalletTypesComboBoxWrapper()
        {
            this.IsEditable = true;
            _ItemsSourceProvider = new PalletTypesComboBoxProvider();
            this.GotFocus += PalletTypesComboBoxWrapper_GotFocus;
        }

        private void PalletTypesComboBoxWrapper_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            this.ItemsSource = _ItemsSourceProvider.GetItems();
        }

        private readonly PalletTypesComboBoxProvider _ItemsSourceProvider;
    }
}
