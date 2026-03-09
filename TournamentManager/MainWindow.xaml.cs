using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Configuration;
using System.Text;
using System.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TournamentManager.ViewModels;
using Wpf.Ui.Controls;

namespace TournamentManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Navigation_PoslovniceClick(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new Poslovnice());
        }

        private void Navigation_TimoviClick(object sender, RoutedEventArgs e) 
        {
            MainContentFrame.Navigate(App.ServiceProvider.GetRequiredService<Timovi>());
        }

        private void Navigation_TurnirClick(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(App.ServiceProvider.GetRequiredService<Turnir>());
        }

        private void MainWindow_Close(object sender, CancelEventArgs e) 
        {
            var result = System.Windows.MessageBox.Show("Sigurno želite izaći?","Exit", System.Windows.MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.No)
            {
                e.Cancel = true; // stops the window from closing
            }
        }
    }
}