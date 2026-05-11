using System.Windows;
using System.Windows.Controls;
using UPShootAndBunny.Pages;

namespace UPShootAndBunny
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new AuthPage());
        }
        private void Go_Catalog(object s, RoutedEventArgs e) { MainFrame.Navigate(new CatalogPage()); }
        private void Go_Lists(object s, RoutedEventArgs e) { MainFrame.Navigate(new ListsPage()); }
        private void Go_Profile(object s, RoutedEventArgs e) { MainFrame.Navigate(new ProfilePage()); }
        private void Go_Author(object s, RoutedEventArgs e) { MainFrame.Navigate(new AuthorPage()); }
        private void Go_Admin(object s, RoutedEventArgs e) { MainFrame.Navigate(new AdminPage()); }
        private void Btn_Logout(object s, RoutedEventArgs e)
        {
            Core.Context = new uchebkaEntities();
            App.CurrentUser = null;
            UpdateSidebar(false);
            MainFrame.Navigate(new AuthPage());
        }
        public void UpdateSidebar(bool active)
        {
            SidebarBorder.Visibility = active ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}