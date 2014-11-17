using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using QueryManager;

namespace PosizioniRoverfrutta.Windows
{
    public abstract class BaseWindow : RibbonWindow
    {
        protected readonly IWindowManager _windowManager;

        protected BaseWindow(IWindowManager windowManager, IDataStorage dataStorage)
        {
            _windowManager = windowManager;
            DataStorage = dataStorage;
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/Content/Pictures/ring-binders.ico"));
        }

        protected void SetButtonBinding(INotifyPropertyChanged viewModel, Button buttonToBind, string propertyName, ICommand command)
        {
            var binding = new CommandBinding
            {
                Command = command
            };
            CommandBindings.Add(binding);

            buttonToBind.SetBinding(ButtonBase.CommandProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath(propertyName)
            });
        }

        public IDataStorage DataStorage { get; private set; }

        public virtual int Index { get; set; }
    }
}
