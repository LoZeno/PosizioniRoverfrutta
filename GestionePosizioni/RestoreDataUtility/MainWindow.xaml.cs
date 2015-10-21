using System.Windows;

namespace RestoreDataUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            textBox.TextChanged += (sender, e) =>
            {
                textBox.ScrollToEnd();
            };
        }
    }
}
