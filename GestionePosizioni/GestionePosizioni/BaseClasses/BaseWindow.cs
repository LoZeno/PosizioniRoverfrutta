using System.Windows;
using System.Windows.Controls.Ribbon;
using QueryManager;
using Raven.Client;

namespace GestionePosizioni.BaseClasses
{
    public class BaseWindow : RibbonWindow
    {
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
