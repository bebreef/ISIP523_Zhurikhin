using System.Windows;
using System.Windows.Controls;

namespace Pr16.Pages
{
    public partial class GameOverPage : Page
    {
        public GameOverPage()
        {
            InitializeComponent();
        }
        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is Pr16.MainWindow mw)
            {
                mw.MainFrame.Navigate(new StartPage());
            }
        }
    }
}