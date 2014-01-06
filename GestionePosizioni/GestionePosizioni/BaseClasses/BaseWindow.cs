using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Ribbon;
using QueryManager;

namespace GestionePosizioni.BaseClasses
{
    public class BaseWindow : RibbonWindow
    {
        public IDataStorage DataStorage
        {
            get { return ((App) Application.Current).DataStorage; }
        }
    }
}
