using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace TournamentManager
{
    public class OutcomeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var team = value as Team;
            if (team == null) return Brushes.Transparent;

            if (team.IsWinner == true) return Brushes.Green;
            if (team.IsLoser == true) return Brushes.Red;
            if (team.IsDraw == true) return Brushes.Yellow;

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
