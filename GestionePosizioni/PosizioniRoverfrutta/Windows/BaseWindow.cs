using System;
using System.Windows.Controls.Ribbon;
using System.Windows.Media.Imaging;
using QueryManager;

namespace PosizioniRoverfrutta.Windows
{
    public abstract class BaseWindow : RibbonWindow
    {
        protected BaseWindow(IDataStorage dataStorage)
        {
            DataStorage = dataStorage;
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/Content/Pictures/ring-binders.ico"));
        }

        public IDataStorage DataStorage { get; private set; }

        public virtual int Index { get; set; }
    }
}
