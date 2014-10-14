using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PosizioniRoverfrutta.CustomControls.AssociatedDataGrid
{
    public static class AssociatedDataGrid
    {
        public static DataGrid GetBottom(DependencyObject obj)
        {
            return (DataGrid)obj.GetValue(BottomProperty);
        }

        public static void SetBottom(DependencyObject obj, DataGrid value)
        {
            obj.SetValue(BottomProperty, value);
        }

        // Using a DependencyProperty as the backing store for BottomDataGrid.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomProperty =
            DependencyProperty.RegisterAttached("Bottom", typeof(DataGrid), typeof(AssociatedDataGrid), new UIPropertyMetadata(null, AssociatedDataGridPropertyChanged));



        public static DataGrid GetRight(DependencyObject obj)
        {
            return (DataGrid)obj.GetValue(RightProperty);
        }

        public static void SetRight(DependencyObject obj, DataGrid value)
        {
            obj.SetValue(RightProperty, value);
        }

        // Using a DependencyProperty as the backing store for RightDataGrid.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightProperty =
            DependencyProperty.RegisterAttached("Right", typeof(DataGrid), typeof(AssociatedDataGrid), new UIPropertyMetadata(null, AssociatedDataGridPropertyChanged));



        public static DataGrid GetLeft(DependencyObject obj)
        {
            return (DataGrid)obj.GetValue(LeftProperty);
        }

        public static void SetLeft(DependencyObject obj, DataGrid value)
        {
            obj.SetValue(LeftProperty, value);
        }

        // Using a DependencyProperty as the backing store for LeftDataGrid.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftProperty =
            DependencyProperty.RegisterAttached("Left", typeof(DataGrid), typeof(AssociatedDataGrid), new UIPropertyMetadata(null, AssociatedDataGridPropertyChanged));



        public static DataGrid GetTop(DependencyObject obj)
        {
            return (DataGrid)obj.GetValue(TopProperty);
        }

        public static void SetTop(DependencyObject obj, DataGrid value)
        {
            obj.SetValue(TopProperty, value);
        }

        // Using a DependencyProperty as the backing store for TopDataGrid.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopProperty =
            DependencyProperty.RegisterAttached("Top", typeof(DataGrid), typeof(AssociatedDataGrid), new UIPropertyMetadata(null, AssociatedDataGridPropertyChanged));



        public static int GetColumnSpan(DependencyObject obj)
        {
            return (int)obj.GetValue(ColumnSpanProperty);
        }

        public static void SetColumnSpan(DependencyObject obj, int value)
        {
            obj.SetValue(ColumnSpanProperty, value);
        }

        // Using a DependencyProperty as the backing store for ColumnSpan.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnSpanProperty =
            DependencyProperty.RegisterAttached("ColumnSpan", typeof(int), typeof(AssociatedDataGrid), new UIPropertyMetadata(1));



        private static double? GetInternalWidthOnColumn(DependencyObject obj)
        {
            return (double)obj.GetValue(InternalWidthOnColumnProperty);
        }

        private static void SetInternalWidthOnColumn(DependencyObject obj, double? value)
        {
            obj.SetValue(InternalWidthOnColumnProperty, value);
        }

        // Using a DependencyProperty as the backing store for InternalWidthOnColumn.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty InternalWidthOnColumnProperty =
            DependencyProperty.RegisterAttached("InternalWidthOnColumn", typeof(double?), typeof(AssociatedDataGrid), new UIPropertyMetadata(null, InternalWidthOnColumnPropertyChanged));


        private static void InternalWidthOnColumnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                ((DataGridColumn)sender).Width = ((double?)e.NewValue).Value;
            }
        }

        private static void SynchronizeHorizontalDataGrid(DataGrid source, DataGrid associated)
        {
            associated.HeadersVisibility = source.HeadersVisibility & (DataGridHeadersVisibility.Column);
            associated.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }

        private static void SynchronizeVerticalDataGrid(DataGrid source, DataGrid associated)
        {
            associated.HeadersVisibility = source.HeadersVisibility & (DataGridHeadersVisibility.Row);
            associated.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;

            int sourceColIndex = 0;

            bool bAllowColumnDisplayIndexSynchronization = true;
            for (int i = 0; i < associated.Columns.Count; i++)
            {
                if (GetColumnSpan(associated.Columns[i]) > 1)
                {
                    bAllowColumnDisplayIndexSynchronization = false;
                    break;
                }
            }


            for (int associatedColIndex = 0; associatedColIndex < associated.Columns.Count; associatedColIndex++)
            {
                var colAssociated = associated.Columns[associatedColIndex];
                int columnSpan = GetColumnSpan(colAssociated);

                if (sourceColIndex >= source.Columns.Count)
                    break;

                if (columnSpan <= 1)
                {
                    var colSource = source.Columns[sourceColIndex];
                    Binding binding = new Binding();
                    binding.Mode = BindingMode.TwoWay;
                    binding.Source = colSource;
                    binding.Path = new PropertyPath(DataGridColumn.WidthProperty);
                    BindingOperations.SetBinding(colAssociated, DataGridColumn.WidthProperty, binding);

                    if (bAllowColumnDisplayIndexSynchronization)
                    {
                        binding = new Binding();
                        binding.Mode = BindingMode.TwoWay;
                        binding.Source = colSource;
                        binding.Path = new PropertyPath(DataGridColumn.DisplayIndexProperty);
                        BindingOperations.SetBinding(colAssociated, DataGridColumn.DisplayIndexProperty, binding);
                    }

                    sourceColIndex++;

                }
                else
                {
                    MultiBinding multiBinding = new MultiBinding();

                    multiBinding.Converter = WidthConverter;
                    for (int i = 0; i < columnSpan; i++)
                    {
                        // 1 binding pour forcer le raffraichissement
                        // + 1 binding pour avoir la colonne source
                        var colSource = source.Columns[sourceColIndex];
                        Binding binding = new Binding();
                        binding.Source = colSource;
                        binding.Path = new PropertyPath(DataGridColumn.WidthProperty);
                        multiBinding.Bindings.Add(binding);
                        binding = new Binding();
                        binding.Source = colSource;
                        multiBinding.Bindings.Add(binding);
                        sourceColIndex++;
                    }
                    // Rq : use another internal Property
                    //      because original Width property is scratch by Framework AFTER binding is set !
                    BindingOperations.SetBinding(colAssociated, InternalWidthOnColumnProperty, multiBinding);

                }
            }
        }

        private static IMultiValueConverter WidthConverter = new WidthConverterClass();
        private class WidthConverterClass : IMultiValueConverter
        {
            public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                double result = 0;
                foreach (var value in values)
                {
                    DataGridColumn column = value as DataGridColumn;
                    if (column != null)
                    {
                        result = result + column.ActualWidth;
                    }
                }
                return result;
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        private static void AssociatedDataGridPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // Gestion des synchonisations des colonnes
            DataGrid dgSource = (DataGrid)sender;

            DataGrid dgBottom = GetBottom(dgSource);
            DataGrid dgTop = GetTop(dgSource);
            DataGrid dgLeft = GetLeft(dgSource);
            DataGrid dgRight = GetRight(dgSource);

            if ((dgBottom == null) &&
                (dgTop == null) &&
                (dgLeft == null) &&
                (dgRight == null))
                dgSource.RemoveHandler(ScrollViewer.ScrollChangedEvent, new RoutedEventHandler(xDataGrid_ScrollChanged));
            else
                dgSource.AddHandler(ScrollViewer.ScrollChangedEvent, new RoutedEventHandler(xDataGrid_ScrollChanged));

            if (dgBottom != null)
            {
                SynchronizeVerticalDataGrid(dgSource, dgBottom);
            }
            if (dgTop != null)
            {
                SynchronizeVerticalDataGrid(dgSource, dgTop);
            }
            if (dgRight != null)
            {
                SynchronizeHorizontalDataGrid(dgSource, dgRight);
            }
            if (dgLeft != null)
            {
                SynchronizeHorizontalDataGrid(dgSource, dgLeft);
            }
        }

        private const string ScrollViewerNameInTemplate = "DG_ScrollViewer";

        private static void xDataGrid_ScrollChanged(object sender, RoutedEventArgs eBase)
        {
            var e = (ScrollChangedEventArgs)eBase;
            // ScrollView source à l'origine du Scroll
            var sourceScrollViewer = (ScrollViewer)e.OriginalSource;

            SynchronizeScrollHorizontalOffset(AssociatedDataGrid.GetBottom((DependencyObject)sender), sourceScrollViewer);
            SynchronizeScrollHorizontalOffset(AssociatedDataGrid.GetTop((DependencyObject)sender), sourceScrollViewer);

            SynchronizeScrollVerticalOffset(AssociatedDataGrid.GetRight((DependencyObject)sender), sourceScrollViewer);
            SynchronizeScrollVerticalOffset(AssociatedDataGrid.GetLeft((DependencyObject)sender), sourceScrollViewer);
        }

        private static void SynchronizeScrollHorizontalOffset(DataGrid associatedDataGrid, ScrollViewer sourceScrollViewer)
        {
            if (associatedDataGrid != null)
            {
                var associatedScrollViewer = (ScrollViewer)associatedDataGrid.Template.FindName(ScrollViewerNameInTemplate, associatedDataGrid);
                associatedScrollViewer.ScrollToHorizontalOffset(sourceScrollViewer.HorizontalOffset);
            }
        }
        private static void SynchronizeScrollVerticalOffset(DataGrid associatedDataGrid, ScrollViewer sourceScrollViewer)
        {
            if (associatedDataGrid != null)
            {
                var associatedScrollViewer = (ScrollViewer)associatedDataGrid.Template.FindName(ScrollViewerNameInTemplate, associatedDataGrid);
                associatedScrollViewer.ScrollToVerticalOffset(sourceScrollViewer.VerticalOffset);
            }
        }
    }
}
