using System;
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
            try
            {
                Convert.ToDecimal(e.Text);
            }
            catch
            {
                // Set handled to true
                e.Handled = true;
            }
        }
    }
}