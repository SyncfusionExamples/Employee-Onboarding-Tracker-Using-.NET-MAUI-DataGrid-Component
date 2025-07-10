using System.Globalization;

namespace DataGridMAUI
{
    public class ProgressConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int percentage)
            {
                return percentage / 100.0;
            }
            return 0;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value;
            //if (value is double progress)
            //{
            //    return (int)(progress * 100);
            //}
            //return 0;
        }
    }
}