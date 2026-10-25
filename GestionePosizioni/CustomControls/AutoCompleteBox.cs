using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CustomWPFControls;

public enum AutoCompleteFilterMode
{
    None,
    StartsWith,
    Contains
}

public class PopulatingEventArgs : EventArgs
{
    public bool Cancel { get; set; }
}

public class AutoCompleteTextBox : AutoCompleteBox;

public class AutoCompleteBox : ComboBox
{
    static AutoCompleteBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoCompleteBox),
            new FrameworkPropertyMetadata(typeof(ComboBox)));
    }

    public AutoCompleteBox()
    {
        IsEditable = true;
        IsTextSearchEnabled = false;
        StaysOpenOnEdit = true;
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(AutoCompleteBox),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged));

    public new string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string SearchText => Text ?? string.Empty;

    public AutoCompleteFilterMode FilterMode { get; set; } = AutoCompleteFilterMode.StartsWith;

    public bool IsTextCompletionEnabled { get; set; }

    public event EventHandler<PopulatingEventArgs>? Populating;

    public void PopulateComplete()
    {
        IsDropDownOpen = ItemsSource != null;
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);
        if (GetTemplateChild("PART_EditableTextBox") is TextBox tb)
            SetCurrentValue(TextProperty, tb.Text);
        RaisePopulating();
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AutoCompleteBox box && box.GetTemplateChild("PART_EditableTextBox") is TextBox tb && tb.Text != box.Text)
            tb.Text = box.Text ?? string.Empty;
    }

    private void RaisePopulating()
    {
        var args = new PopulatingEventArgs();
        Populating?.Invoke(this, args);
        if (!args.Cancel)
            PopulateComplete();
    }
}
