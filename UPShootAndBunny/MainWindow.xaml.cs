using System.Windows;
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

        private void Go_Catalog(object s, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CatalogPage());
        }

        private void Go_Lists(object s, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ListsPage());
        }

        private void Go_Profile(object s, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProfilePage());
        }

        private void Go_Author(object s, RoutedEventArgs e)
        {
            if (App.CurrentUser == null || App.CurrentUser.RoleId != 2)
            {
                MessageBox.Show("Доступ разрешен только авторам", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MainFrame.Navigate(new AuthorPage());
        }

        private void Go_Admin(object s, RoutedEventArgs e)
        {
            if (App.CurrentUser == null || App.CurrentUser.RoleId != 3)
            {
                MessageBox.Show("Доступ разрешен только администраторам", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MainFrame.Navigate(new AdminPage());
        }

        private void Btn_Logout(object s, RoutedEventArgs e)
        {
            Core.Context = new uchebkaEntities();
            App.CurrentUser = null;
            UpdateSidebar(false);
            MainFrame.Navigate(new AuthPage());
        }

        public void UpdateSidebar(bool isLoggedIn)
        {
            if (!isLoggedIn)
            {
                SidebarBorder.Visibility = Visibility.Collapsed;
                BtnAuthor.Visibility = Visibility.Collapsed;
                BtnAdmin.Visibility = Visibility.Collapsed;
                MainFrame.Navigate(new AuthPage());
                return;
            }

            SidebarBorder.Visibility = Visibility.Visible;
            BtnAuthor.Visibility = App.CurrentUser != null && App.CurrentUser.RoleId == 2 ? Visibility.Visible : Visibility.Collapsed;
            BtnAdmin.Visibility = App.CurrentUser != null && App.CurrentUser.RoleId == 3 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}