using System.Windows;

namespace PosizioniRoverfrutta.Windows
{
    public interface IWindowManager
    {
        void InstantiateWindow(string documentId, WindowTypes windowType);
        string OpenSaveToPdfDialog(Window ownerWindow, string filename);
    }
}