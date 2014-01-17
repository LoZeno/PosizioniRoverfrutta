using System.Windows;
using System.Windows.Controls;
using QueryManager;
using Raven.Client;

namespace GestionePosizioni.BaseClasses
{
    public class BaseUserControl : UserControl
    {
        private IDocumentSession _databaseSession;

        protected IDataStorage DataStorage
        {
            get { return ((App)Application.Current).DataStorage; }
        }

        protected IDocumentSession DatabaseSession
        {
            get
            {
                Window parentWindow = Window.GetWindow(this);
                BaseWindow myParent = parentWindow as BaseWindow;
                if (myParent != null)
                {
                    _databaseSession = myParent.DatabaseSession;
                }
                else
                {
                    if (_databaseSession == null)
                        _databaseSession = DataStorage.DocumentStore.OpenSession();
                }
                return _databaseSession;
            }
        }

        protected IDocumentSession GetDatabaseSession()
        {
            return DataStorage.DocumentStore.OpenSession();
        }
    }
}
