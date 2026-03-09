using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TournamentManager
{
    public class ElementVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool isViewingWinners = (bool)values[0];
            int roundCount = (int)values[1];

            return parameter switch
            {
                "NoRound" => roundCount == 0 ? Visibility.Visible : Visibility.Collapsed,
                "FirstRound" => roundCount == 1 ? Visibility.Visible : Visibility.Collapsed,
                "WinnersBracket" => roundCount > 1 && isViewingWinners ? Visibility.Visible : Visibility.Collapsed,
                "LosersBracket" => roundCount > 1 && !isViewingWinners ? Visibility.Visible : Visibility.Collapsed,
                _ => Visibility.Collapsed
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
