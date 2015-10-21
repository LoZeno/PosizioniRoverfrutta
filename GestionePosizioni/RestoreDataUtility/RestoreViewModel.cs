using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RestoreDataUtility
{
    public class RestoreViewModel : INotifyPropertyChanged
    {
        public RestoreViewModel()
        {
            Output = new ObservableCollection<string>();
        }

        public ICommand StartRestore
        { get { return _startRestoreCommand ?? (_startRestoreCommand = new DelegateCommand(StartRestoreCommand)); } }

        public ObservableCollection<string> Output { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void StartRestoreCommand()
        {
            var selectedPath = OpenSelectFolderDialog();
            if (!string.IsNullOrWhiteSpace(selectedPath) && Directory.Exists(selectedPath))
            {
                if (CheckIfContainsBackup(selectedPath))
                {
                    try
                    {
                        if (Directory.Exists(_dataDirectory))
                        {
                            File.SetAttributes(_dataDirectory, FileAttributes.Normal);
                            setAttributesNormal(_dataDirectory);
                            Directory.Delete(_dataDirectory, true);
                        }

                        await Task.Run(() => Raven.Database.DocumentDatabase.Restore(new Raven.Database.Config.RavenConfiguration { DataDirectory = _dataDirectory }, selectedPath, _dataDirectory, WriteOutput, true));
                        MessageBox.Show("Ripristino dei dati completato", "Ripristino completato", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception error)
                    {
                        MessageBox.Show(error.Message, "Eccezione non gestita", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("La cartella selezionata non contiene un backup dei dati valido, o il backup e` corrotto.", "La cartella non contiene un backup valido", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
        }

        private void WriteOutput(string outputLine)
        {
            Output.AsyncAdd(outputLine);
        }

        private string OpenSelectFolderDialog()
        {
            var savefileDialog = new SaveFileDialog()
            {
                FileName = "Seleziona la cartella contenente il backup",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                Title = "Seleziona la cartella",
            };
            var result = savefileDialog.ShowDialog();

            return result == true ? Path.GetDirectoryName(savefileDialog.FileName) : null;
        }

        private bool CheckIfContainsBackup(string selectedPath)
        {
            var rootContent = Directory.GetFiles(selectedPath);
            if (!(rootContent.Any(x => x.EndsWith("RavenDB.Backup")) && rootContent.Any(x => x.EndsWith("Database.Document")) && rootContent.Any(x => x.EndsWith(".all-existing-index-files"))))
            {
                return false;
            }
            var subFolders = Directory.GetDirectories(selectedPath);
            if (!(subFolders.Any( x => x.Contains("Indexes")) && subFolders.Any(x => x.Contains("IndexDefinitions")) && subFolders.Any(x => x.Contains("new"))))
            {
                return false;
            }
            var indexDefinitionFolder = Path.Combine(selectedPath, "IndexDefinitions");
            var indexDefinitionFiles = Directory.GetFiles(indexDefinitionFolder);
            if (indexDefinitionFolder.Length == 0)
            {
                return false;
            }
            var indexesFolder = Path.Combine(selectedPath, "Indexes");
            var indexesSubfolders = Directory.GetDirectories(indexesFolder);
            if (indexesSubfolders.Length != rootContent.Count(x => x.EndsWith(".all-existing-index-files")))
            {
                return false;
            }
            var newFolder = Path.Combine(selectedPath, "new");
            var newFolderContent = Directory.GetFiles(newFolder);
            if (!(newFolderContent.Any( x => x.EndsWith("Data")) && newFolderContent.Any(x => x.EndsWith("Data.pat")) && newFolderContent.Any(x => x.EndsWith(".log"))))
            {
                return false;
            }
            return true;
        }

        private void setAttributesNormal(string dir)
        {
            foreach (string subDirPath in Directory.GetDirectories(dir, "*", SearchOption.AllDirectories))
                setAttributesNormal(subDirPath);
            foreach (string filePath in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
            }
        }

        private string _dataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Posizioni\Archive");
        private ICommand _startRestoreCommand;
    }

    public static class ObservableCollectionExtensions
    {
        public static void AsyncAdd<T>(this ICollection<T> collection, T item)
        {
            Action<T> addMethod = collection.Add;
            Application.Current.Dispatcher.BeginInvoke(addMethod, item);
        }
    }
}
