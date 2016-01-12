using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PosizioniRoverfrutta.ViewModels.Statistics;
using QueryManager;

namespace PosizioniRoverfrutta.Windows.Statistics
{
    /// <summary>
    /// Logica di interazione per CustomerStatistics.xaml
    /// </summary>
    public partial class CustomerStatistics : BaseWindow
    {
        public CustomerStatistics()
            : this(null, null, null)
        {
            
        }

        public CustomerStatistics(IWindowManager windowManager, IDataStorage dataStorage, string customerId)
        {
            InitializeComponent();
            var viewModel = new CustomerStatisticsViewModel(dataStorage, customerId);
            this.DataContext = viewModel;
        }
    }
}
