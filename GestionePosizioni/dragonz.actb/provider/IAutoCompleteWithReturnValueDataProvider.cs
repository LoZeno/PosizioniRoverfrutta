using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dragonz.actb.provider
{
    public class AutoCompleteSelectedEventArgs : EventArgs
    {
        private object _selectedValue;

        public AutoCompleteSelectedEventArgs(object value)
        {
            _selectedValue = value;
        }

        public object GetSelectedValue()
        {
            return _selectedValue;
        }
    }

    public interface IAutoCompleteWithReturnValueDataProvider : IAutoCompleteDataProvider
    {
        object GetValue(string selectedText);
    }
}
