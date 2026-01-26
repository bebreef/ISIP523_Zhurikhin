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

namespace Pr13
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<Page> _pages;
        private int _currentPageIndex = 0;

        public MainWindow()
        {
            InitializeComponent();

            _pages = new List<Page>
        {
            new Pages.ProductsPage(),
            new Pages.OrdersPage(),
            new Pages.ConfirmPage()
        };

            NavigateToPage(0);
        }

        private void NavigateToPage(int index)
        {
            _currentPageIndex = index;
            MainFrame.Navigate(_pages[index]);

            UpdateButtons();
            UpdateTitle();
        }

        private void UpdateButtons()
        {
            BackButton.IsEnabled = _currentPageIndex > 0;
            ForwardButton.IsEnabled = _currentPageIndex < _pages.Count - 1;
        }

        private void UpdateTitle()
        {
            if (_pages[_currentPageIndex] is Page page)
            {
                TitleTextBlock.Text = page.Title;
                Title = $"Магазин – {page.Title}";
            }
        }
        public void SetNextButtonEnabled(bool isEnabled)
        {
            ForwardButton.IsEnabled = isEnabled;
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_currentPageIndex > 0)
                NavigateToPage(_currentPageIndex - 1);
        }

        private void ForwardButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_currentPageIndex < _pages.Count - 1)
                NavigateToPage(_currentPageIndex + 1);
        }
    }
}