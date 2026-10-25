using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Prism.Commands;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations.Backups;
using Raven.Client.ServerWide.Operations;

namespace RestoreDataUtility;

public class RestoreViewModel : INotifyPropertyChanged
{
    public ICommand StartRestore =>
        _startRestoreCommand ??= new DelegateCommand(StartRestoreCommand);

    public string Output
    {
        get => _outPut;
        set
        {
            _outPut = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void StartRestoreCommand()
    {
        var selectedPath = OpenSelectFolderDialog();
        if (string.IsNullOrWhiteSpace(selectedPath) || !Directory.Exists(selectedPath))
            return;

        if (!CheckIfContainsBackup(selectedPath))
        {
            MessageBox.Show("La cartella selezionata non contiene un backup dei dati valido, o il backup e` corrotto.",
                "La cartella non contiene un backup valido", MessageBoxButton.OK, MessageBoxImage.Stop);
            return;
        }

        try
        {
            using var store = new DocumentStore
            {
                Urls = ["http://127.0.0.1:8080"],
                Database = "Archive"
            };
            store.Initialize();
            store.Maintenance.Server.Send(new RestoreBackupOperation(new RestoreBackupConfiguration
            {
                BackupLocation = selectedPath,
                DatabaseName = "Archive",
                DataDirectory = _dataDirectory
            }));
            MessageBox.Show("Ripristino dei dati completato", "Ripristino completato", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception error)
        {
            MessageBox.Show(error.Message, "Eccezione non gestita", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private string OpenSelectFolderDialog()
    {
        var savefileDialog = new SaveFileDialog
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
        return rootContent.Length > 0 || Directory.GetDirectories(selectedPath).Length > 0;
    }

    private readonly string _dataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Posizioni\Archive");
    private ICommand _startRestoreCommand;
    private string _outPut = string.Empty;
}
