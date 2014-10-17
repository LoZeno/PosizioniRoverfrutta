using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace PosizioniRoverfrutta.CustomControls.DataGridColumns
{
    public class DataGridDecimalColumn : DataGridTextColumn
    {
        protected override object PrepareCellForEdit(System.Windows.FrameworkElement editingElement, System.Windows.RoutedEventArgs editingEventArgs)
        {
            var edit = editingElement as TextBox;
            edit.PreviewTextInput += OnPreviewTextInput;

            return base.PrepareCellForEdit(editingElement, editingEventArgs);
        }

        void OnPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^\d([.,]\d{1,3})?$"))
            {
                e.Handled = true;
            }

            //try
            //{
            //    Convert.ToDouble(e.Text);
            //}
            //catch
            //{
            //    // Set handled to true
            //    e.Handled = true;
            //}
        }
    }
}