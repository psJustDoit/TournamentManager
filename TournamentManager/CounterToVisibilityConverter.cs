using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TournamentManager
{
    public class CounterToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int roundCount = (int)value;
            string mode = parameter.ToString();

            return mode switch
            {
                "0" => roundCount == 0 ? Visibility.Visible : Visibility.Collapsed,
                ">=1" => roundCount > 0 ? Visibility.Visible : Visibility.Collapsed,
                _ => Visibility.Collapsed
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();

    }
}
