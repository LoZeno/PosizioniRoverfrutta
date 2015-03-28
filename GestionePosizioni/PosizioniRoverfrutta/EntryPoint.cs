using System;
using System.Threading;
using System.Windows;

namespace PosizioniRoverfrutta
{
    public class EntryPoint
    {
        private static Mutex mutex = new Mutex(true, "Roverfrutta_ArchiveManager");

        [STAThread]
        static void Main(string[] args)
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                var splash = new SplashScreen("content/Pictures/archivio.png");
                splash.Show(true);
                App.Main();
                mutex.ReleaseMutex();
            }
        }
    }
}
