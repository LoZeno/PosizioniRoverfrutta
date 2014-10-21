using QueryManager;

namespace PosizioniRoverfrutta.Windows
{
    /// <summary>
    /// Interaction logic for SummaryAndInvoicesWindow.xaml
    /// </summary>
    public partial class SummaryAndInvoicesWindow : BaseWindow
    {
        public SummaryAndInvoicesWindow()
            : this(null, null)
        {
            
        }

        public SummaryAndInvoicesWindow(IWindowManager windowManager, IDataStorage dataStorage)
            :base(windowManager, dataStorage)
        {
            InitializeComponent();
        }

        public SummaryAndInvoicesWindow(IWindowManager windowManager, IDataStorage dataStorage, string documentId)
            : this(windowManager, dataStorage)
        {
            
        }
    }
}
