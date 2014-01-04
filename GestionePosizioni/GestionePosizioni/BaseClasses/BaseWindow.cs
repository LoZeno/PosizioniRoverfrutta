using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using QueryManager;

namespace GestionePosizioni.BaseClasses
{
    public class BaseWindow : Window
    {
        public IDataStorage DataStorage
        {
            get { return ((App) Application.Current).DataStorage; }
        }
    }
}
