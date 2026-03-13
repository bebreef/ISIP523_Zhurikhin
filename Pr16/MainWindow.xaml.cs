using System.Windows;
using Pr16.Pages;

namespace Pr16
{
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            MainFrame.Navigate(new StartPage());
        }
    }
}