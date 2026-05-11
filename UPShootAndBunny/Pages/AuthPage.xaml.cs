using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UPShootAndBunny.Pages
{
    public partial class AuthPage : Page
    {
        public AuthPage() { InitializeComponent(); }
        private void Btn_Login(object s, RoutedEventArgs e)
        {
            try
            {
                string login = TbLogin.Text.Trim();
                string pass = PbPass.Password;
                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(pass)) { ShowErr("Заполните все поля"); return; }
                var user = Core.Context.Users.FirstOrDefault(u => u.Login == login && u.PasswordHash == pass);
                if (user == null) { ShowErr("Неверные данные"); return; }
                if (user.IsFrozen) { ShowErr("Аккаунт заморожен"); return; }
                App.CurrentUser = user;
                ((MainWindow)Application.Current.MainWindow).UpdateSidebar(true);
                ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new CatalogPage());
            }
            catch (Exception ex) { MessageBox.Show($"Ошибка: {ex.Message}", "Сбой", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
        private void Btn_Register(object s, RoutedEventArgs e)
        {
            try
            {
                string login = TbLogin.Text.Trim();
                string pass = PbPass.Password;
                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(pass)) { ShowErr("Заполните все поля"); return; }
                if (Core.Context.Users.Any(x => x.Login == login)) { ShowErr("Логин занят"); return; }
                var newUser = new Users { Login = login, PasswordHash = pass, Email = login + "@mail.ru", DisplayName = login, RoleId = 1, IsFrozen = false, CreatedAt = DateTime.Now };
                Core.Context.Users.Add(newUser);
                Core.Context.SaveChanges();
                App.CurrentUser = newUser;
                ((MainWindow)Application.Current.MainWindow).UpdateSidebar(true);
                ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new CatalogPage());
            }
            catch (Exception ex) { MessageBox.Show($"Ошибка: {ex.Message}", "Сбой", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
        private void Btn_Exit(object s, RoutedEventArgs e) { Application.Current.Shutdown(); }
        private void ShowErr(string msg) { TbError.Text = msg; TbError.Visibility = Visibility.Visible; }
    }
}