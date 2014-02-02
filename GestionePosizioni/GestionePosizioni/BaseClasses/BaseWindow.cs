using System;
using System.Windows.Controls.Ribbon;
using System.Windows.Media.Imaging;
using QueryManager;
using Raven.Client;

namespace GestionePosizioni.BaseClasses
{
    public class BaseWindow : RibbonWindow
    {
        public BaseWindow()
        {
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/Content/Pictures/ring-binders.ico"));
        }

        private IDocumentSession _databaseSession;

        public IDataStorage DataStorage
        {
            get { return Bootstrapper.Instance.Resolve<IDataStorage>(); }
        }

        public IDocumentSession DatabaseSession
        {
            get
            {
                if (_databaseSession == null)
                    _databaseSession = DataStorage.DocumentStore.OpenSession();
                return _databaseSession;
            }
        }
    }
}
