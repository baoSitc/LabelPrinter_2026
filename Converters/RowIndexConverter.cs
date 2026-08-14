using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace LabelPrinter.Converters
{
    public class RowIndexConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value is DataGridRow row)
            {
                var dataGrid = ItemsControl.ItemsControlFromItemContainer(row)
                    as DataGrid;

                if (dataGrid != null)
                {
                    return dataGrid.Items.IndexOf(row.Item) + 1;
                }
            }

            return "";
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
