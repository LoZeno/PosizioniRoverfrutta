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
    /// Interaction logic for InvoicesWindow.xaml
    /// </summary>
    public partial class InvoicesWindow : BaseWindow
    {
        public InvoicesWindow()
            : this(null, null)
        {
            
        }

        public InvoicesWindow(IWindowManager windowManager, IDataStorage dataStorage, string documentId)
            : this(windowManager, dataStorage)
        {
            
        }

        public InvoicesWindow(IWindowManager windowManager, IDataStorage dataStorage)
            : base(windowManager, dataStorage)
        {
            InitializeComponent();
        }
    }
}
