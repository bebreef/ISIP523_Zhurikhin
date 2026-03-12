using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Pr16.Pages;

namespace Pr16
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            StartMenuGrid.Visibility = Visibility.Visible;
            MainFrame.Content = null;
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            StartMenuGrid.Visibility = Visibility.Collapsed;
            var page = new GamePage();
            MainFrame.Content = page;

            if (page.DataContext is ViewModels.GameViewModel vm)
            {
                vm.StartNewGame();
            }
        }
    }
}