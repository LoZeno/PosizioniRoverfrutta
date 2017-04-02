using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using QueryManager;

namespace PosizioniRoverfrutta.Windows
{
    public class WindowManager : IWindowManager
    {
        private readonly IDataStorage _dataStorage;
        private readonly Dictionary<string, Window> _windows;
        private readonly Dictionary<WindowTypes, Type> _windowClasses;
        private static string _attachmentsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
    @"Posizioni\Allegati");

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
            string key = String.Format("{0}_{1}", windowType, documentId.Replace('/', '_'));
            ManageWindows(key, _windowClasses[windowType], documentId);
        }

        public void PopupMessage(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public string OpenSaveToPdfDialog(string filename)
        {
            return OpenSaveFileDialog(filename, ".pdf", "Documenti PDF (.pdf)|*.pdf");
        }

        public string OpenSelectFolderDialog()
        {
            var savefileDialog = new SaveFileDialog
            {
                FileName = "Seleziona la cartella",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
            };
            var result = savefileDialog.ShowDialog();

            return result == true ? Path.GetDirectoryName(savefileDialog.FileName) : null;
        }

        public void OpenAttachmentWindow(int positionNumber)
        {
            var documentAttachmentsPath = Path.Combine(_attachmentsDirectory, positionNumber.ToString());
            Directory.CreateDirectory(documentAttachmentsPath);
            Process.Start(documentAttachmentsPath);
        }

        private string OpenSaveFileDialog(string filename, string extension, string filter)
        {
            var savefileDialog = new SaveFileDialog
            {
                FileName = filename,
                DefaultExt = extension,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = filter
            };
            var result = savefileDialog.ShowDialog();

            return result == true ? savefileDialog.FileName : null;
        }

        private void ManageWindows(string windowName, Type windowType, string documentId)
        {
            if (!WindowIsAlreadyOpen(windowName))
            {
                //caricare il viewmodel corrispondente?
                InstantiateNewWindow(windowName, windowType, documentId);
            }
        }

        private void InstantiateNewWindow(string key, Type windowType, string documentId)
        {
            var window = (Window)Activator.CreateInstance(windowType, this, _dataStorage, documentId);
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
            if (_windows[name].WindowState == WindowState.Minimized)
            {
                _windows[name].WindowState = WindowState.Normal;
            }
            _windows[name].Activate();
            return true;
        }
    }

    public enum WindowTypes
    {
        ConfermaVendita,
        DistintaCarico,
        ConfermaPrezzi,
        Riepiloghi,
        AnagraficaClienti,
        AnagraficaTrasportatori,
        AnagraficaProdotti,
        StatisticheClienti
    }
}
