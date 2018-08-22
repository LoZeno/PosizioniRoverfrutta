using PosizioniRoverfrutta.Services;
using QueryManager;

namespace PosizioniRoverfrutta.Windows.Statistics
{
    /// <summary>
    /// Interaction logic for ProductsStatistics.xaml
    /// </summary>
    public partial class ProductsStatistics : BaseWindow
    {
        public ProductsStatistics()
            : this(null, null)
        {
            
        }

        public ProductsStatistics(IWindowManager windowManager, IDataStorage dataStorage, string documentId)
            : this(windowManager, dataStorage)
        {

        }

        public ProductsStatistics(IWindowManager windowManager, IDataStorage dataStorage)
            : base(windowManager, dataStorage)
        {
            InitializeComponent();
            var autocompleteProvider = new ProductNamesAutoCompleteBoxProvider(dataStorage);
            ProductNameBox.DataProvider = autocompleteProvider;
        }
    }
}
