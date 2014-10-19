using System;
using System.Windows.Controls.Ribbon;
using System.Windows.Media.Imaging;
using QueryManager;

namespace PosizioniRoverfrutta.Windows
{
    public abstract class BaseWindow : RibbonWindow
    {
        protected readonly IWindowManager _windowManager;

        protected BaseWindow(IWindowManager windowManager, IDataStorage dataStorage)
        {
            _windowManager = windowManager;
            DataStorage = dataStorage;
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/Content/Pictures/ring-binders.ico"));
        }

        public IDataStorage DataStorage { get; private set; }

        public virtual int Index { get; set; }
    }
}
