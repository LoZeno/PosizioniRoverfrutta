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
using QueryManager;

namespace PosizioniRoverfrutta.Windows
{
    /// <summary>
    /// Interaction logic for PriceConfirmationWindow.xaml
    /// </summary>
    public partial class PriceConfirmationWindow : BaseWindow
    {
        public PriceConfirmationWindow()
            : this(null, null)
        {
            
        }

        public PriceConfirmationWindow(IWindowManager windowManager, IDataStorage dataStorage)
            : base(windowManager, dataStorage)
        {
            InitializeComponent();
        }
    }
}
