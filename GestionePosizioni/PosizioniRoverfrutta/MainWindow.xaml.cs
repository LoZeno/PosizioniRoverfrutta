using System.Windows;
using PosizioniRoverfrutta.Windows;

namespace PosizioniRoverfrutta
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void NewDocumentButton_OnClick(object sender, RoutedEventArgs e)
        {
            var documentWindow = new DocumentWindow();
            documentWindow.Show();
        }
    }
}
