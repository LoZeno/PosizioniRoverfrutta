using System;
using System.Windows;
using System.Windows.Controls.Ribbon;
using System.Windows.Media.Imaging;
using QueryManager;
using Raven.Client;

namespace GestionePosizioni.BaseClasses
{
    public class BaseWindow : RibbonWindow
    {
        public BaseWindow() : base()
        {
            this.Icon = new BitmapImage(new Uri(@"pack://application:,,,/Content/Pictures/ring-binders.ico"));
        }

        private IDocumentSession _databaseSession;

        public IDataStorage DataStorage
        {
            get { return ((App) Application.Current).DataStorage; }
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
