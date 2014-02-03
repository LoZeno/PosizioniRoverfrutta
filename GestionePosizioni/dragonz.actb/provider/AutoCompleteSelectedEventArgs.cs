using System;

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
}