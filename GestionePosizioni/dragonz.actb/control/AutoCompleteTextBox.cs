using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using dragonz.actb.core;
using dragonz.actb.provider;

namespace dragonz.actb.control
{
    public class AutoCompleteTextBox : TextBox
    {
        private AutoCompleteManager _acm;

        public AutoCompleteManager AutoCompleteManager
        {
            get { return _acm; }
        }

        public AutoCompleteTextBox()
        {
            _acm = new AutoCompleteManager();
            _acm.DataProvider = new FileSysDataProvider();
            _acm.OnSelectedItem += _acm_OnSelectedItem;
            this.Loaded += AutoCompleteTextBox_Loaded;
            _acm.SearchComplete += _acm_SearchComplete;
        }

        void _acm_OnSelectedItem(object source, AutoCompleteSelectedEventArgs e)
        {
            object selectedItem = e.GetSelectedValue();
            this.SelectedItem = selectedItem;
        }

        void _acm_SearchComplete(object sender, IEnumerable<string> e)
        {
            this.DataContext = e;
        }

        void AutoCompleteTextBox_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _acm.AttachTextBox(this);
        }

        //public object SelectedItem
        //{
        //    get { return _acm.SelectedItem; }
        //    set { _acm.SelectedItem = value; }
        //}

        public object SelectedItem
        {
            get { return (object)this.GetValue(SelectedItemProperty); }
            set { this.SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
    "SelectedItem", typeof(object), typeof(AutoCompleteTextBox), new PropertyMetadata(false));
    }
}
