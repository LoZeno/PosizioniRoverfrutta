using System;
using System.Collections.Generic;
using System.Windows;
using QueryManager;

namespace PosizioniRoverfrutta.Windows
{
    public class WindowManager
    {
        private readonly IDataStorage _dataStorage;
        private readonly Dictionary<string, Window> _windows;
        private readonly Dictionary<WindowTypes, Type> _windowClasses; 

        public WindowManager(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
            _windows = new Dictionary<string, Window>();
            _windowClasses = new Dictionary<WindowTypes, Type>();
        }

        public void RegisterWindowClass(WindowTypes key, Type windowType)
        {
            if (_windowClasses.ContainsKey(key))
            {
                _windowClasses[key] = windowType;
            }
            else
            {
                _windowClasses.Add(key, windowType);
            }
        }

        public void InstantiateWindow(string documentId, WindowTypes windowType)
        {
            if (!_windowClasses.ContainsKey(windowType))
            {
                return;
            }
            var key = String.Format("{0}_{1}", windowType, documentId);
            ManageWindows(key, _windowClasses[windowType]);
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
            var window = (Window)Activator.CreateInstance(windowType, _dataStorage);
            window.Name = key;
            window.Closed += window_Closed;
            _windows.Add(window.Name, window);
            window.Show();
        }

        void window_Closed(object sender, EventArgs e)
        {
            var key = ((Window)sender).Name;
            if (_windows.ContainsKey(key))
            {
                _windows.Remove(key);
            }
        }

        private bool WindowIsAlreadyOpen(string name)
        {
            if (!_windows.ContainsKey(name)) return false;
            _windows[name].Activate();
            return true;
        }
    }

    public enum WindowTypes
    {
        ConfermaVendita,
        DistintaCarico,
        ConfermaPrezzi
    }
}
