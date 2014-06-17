using System;
using System.Collections.Generic;
using System.Windows;
using QueryManager;

namespace PosizioniRoverfrutta.Windows
{
    internal class WindowManager
    {
        private readonly IDataStorage _dataStorage;
        private readonly Dictionary<string, Window> _windows;

        public WindowManager(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
            _windows = new Dictionary<string, Window>();
        }

        public void InstantiateWindow(string documentId, WindowTypes windowType)
        {
            Window window = null;
            string key = null;
            switch (windowType)
            {
                case WindowTypes.ConfermaPrezzi:
                    key = "confermaprezzi_" + documentId;
                    ManageWindows(key, typeof(DocumentWindow));
                    break;
                case WindowTypes.DistintaCarico:
                    key = "distintacarico_" + documentId;
                    ManageWindows(key, typeof(DocumentWindow));
                    break;
                case WindowTypes.ConfermaVendita:
                    key = "confermavendita_" + documentId;
                    ManageWindows(key, typeof(DocumentWindow));
                    break;
            }
        }

        private void ManageWindows(string windowName, Type windowType)
        {
            if (!WindowIsAlreadyOpen(windowName))
            {
                //caricare il viewmodel corrispondente
                InstantiateNewWindow(windowName, windowType);
            }
        }

        private void InstantiateNewWindow(string key, Type windowType)
        {
            var window = (Window)Activator.CreateInstance(windowType);
            window.Name = key;
            window.Closing += window_Closing;
            _windows.Add(window.Name, window);
            window.Show();
        }

        private bool WindowIsAlreadyOpen(string name)
        {
            if (!_windows.ContainsKey(name)) return false;
            _windows[name].Activate();
            return true;
        }

        void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var key = ((Window) sender).Name;
            if (_windows.ContainsKey(key))
            {
                _windows.Remove(key);
            }
        }
    }

    internal enum WindowTypes
    {
        ConfermaVendita,
        DistintaCarico,
        ConfermaPrezzi
    }
}
