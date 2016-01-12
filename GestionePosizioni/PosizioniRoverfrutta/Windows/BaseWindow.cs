using System;
using System.ComponentModel;
using System.Globalization;
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
        protected readonly IWindowManager WindowManager;

#if DEBUG
        protected BaseWindow() :this (null, null)
        {
            
        }
#endif

        protected BaseWindow(IWindowManager windowManager, IDataStorage dataStorage)
        {
            WindowManager = windowManager;
            DataStorage = dataStorage;
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/Content/Pictures/ring-binders.ico"));
        }

        protected static void SetBindingsForTextBox(string property, TextBox control)
        {
            var binding = new Binding(property)
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            control.SetBinding(TextBox.TextProperty, binding);
        }

        protected static void SetBindingsForTextBlock(string property, TextBlock control)
        {
            var binding = new Binding(property)
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.Default
            };
            control.SetBinding(TextBlock.TextProperty, binding);
        }

        protected static void SetBindingsForComboBox(string property, ComboBox control)
        {
            var binding = new Binding(property)
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            control.SetBinding(ComboBox.TextProperty, binding);
        }

        protected static void SetBindingsForNumericTextBox(string property, TextBox control)
        {
            var binding = new Binding(property)
            {
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus,
                Mode = BindingMode.TwoWay,
                ConverterCulture = CultureInfo.CurrentCulture
            };
            binding.ValidationRules.Add(new ExceptionValidationRule());
            control.SetBinding(TextBox.TextProperty, binding);
        }

        protected static void SetBindingsForDatePickers(string property, DatePicker datePicker)
        {
            var dateBinding = new Binding(property)
            {
                UpdateSourceTrigger = UpdateSourceTrigger.Default,
                Mode = BindingMode.TwoWay
            };
            datePicker.SetBinding(DatePicker.SelectedDateProperty, dateBinding);
        }

        protected static void SetBindingsForTotals(string propertyName, TextBlock textBlock)
        {
            var totalsBinding = new Binding(propertyName)
            {
                UpdateSourceTrigger = UpdateSourceTrigger.Default,
                Mode = BindingMode.OneWay
            };

            textBlock.SetBinding(TextBlock.TextProperty, totalsBinding);
        }

        protected static void SetBindingsForDecimalTotals(string propertyName, TextBlock textBlock)
        {
            var totalsBinding = new Binding(propertyName)
            {
                UpdateSourceTrigger = UpdateSourceTrigger.Default,
                Mode = BindingMode.OneWay,
                StringFormat = "{0:0.##}",
                ConverterCulture = CultureInfo.CurrentCulture
            };

            textBlock.SetBinding(TextBlock.TextProperty, totalsBinding);
        }

        protected static void SetBindingsForPriceTotals(string propertyName, TextBlock textBlock)
        {
            var totalsBinding = new Binding(propertyName)
            {
                UpdateSourceTrigger = UpdateSourceTrigger.Default,
                Mode = BindingMode.OneWay,
                StringFormat = "F2",
                ConverterCulture = CultureInfo.CurrentCulture
            };

            textBlock.SetBinding(TextBlock.TextProperty, totalsBinding);
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
        
        protected void SetMenuItemBinding(INotifyPropertyChanged viewModel, RibbonMenuItem buttonToBind, string propertyName, ICommand command)
        {
            var binding = new CommandBinding
            {
                Command = command
            };
            CommandBindings.Add(binding);

            buttonToBind.SetBinding(MenuItem.CommandProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath(propertyName)
            });
        }

        public IDataStorage DataStorage { get; private set; }

        public virtual int Index { get; set; }
    }
}
