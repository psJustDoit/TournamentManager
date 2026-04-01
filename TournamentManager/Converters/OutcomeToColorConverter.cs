using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TournamentManager.Models;
using TournamentManager.Enums;

namespace TournamentManager.Converters
{
    public class OutcomeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var team = value as Team;
            if (team == null) return Brushes.Transparent;

            if (team.TeamMatchOutcomePrevious == TeamMatchOutcomeEnum.Winner) return Brushes.Green;
            if (team.TeamMatchOutcomePrevious == TeamMatchOutcomeEnum.Loser) return Brushes.Red;
            if (team.TeamMatchOutcomePrevious == TeamMatchOutcomeEnum.Draw) return Brushes.Yellow;

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
