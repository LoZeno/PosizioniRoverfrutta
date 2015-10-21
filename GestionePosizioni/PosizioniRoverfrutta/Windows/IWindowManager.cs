using System.Windows;

namespace PosizioniRoverfrutta.Windows
{
    public interface IWindowManager
    {
        void InstantiateWindow(string documentId, WindowTypes windowType);
        void PopupMessage(string message, string caption);
        string OpenSaveToPdfDialog(string filename);
        string OpenSelectFolderDialog();
    }
}