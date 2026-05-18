using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UPShootAndBunny.Pages
{
    public partial class AuthPage : Page
    {
        private Users _frozenUser;

        public AuthPage()
        {
            InitializeComponent();
        }

        private void Btn_Login(object s, RoutedEventArgs e)
        {
            try
            {
                string login = TbLogin.Text.Trim();
                string password = PbPass.Password;

                if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                {
                    ShowError("Заполните все поля");
                    return;
                }

                var user = Core.Context.Users.FirstOrDefault(u => u.Login == login && u.PasswordHash == password);
                if (user == null)
                {
                    _frozenUser = null;
                    UnfreezePanel.Visibility = Visibility.Collapsed;
                    ShowError("Неверный логин или пароль");
                    return;
                }

                if (user.IsFrozen)
                {
                    _frozenUser = user;
                    UnfreezePanel.Visibility = Visibility.Visible;
                    return;
                }

                App.CurrentUser = user;
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.UpdateSidebar(true);
                mainWindow.MainFrame.Navigate(new CatalogPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка входа: " + ex.Message);
            }
        }

        private void Btn_Register(object s, RoutedEventArgs e)
        {
            try
            {
                string login = TbLogin.Text.Trim();
                string password = PbPass.Password;

                if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                {
                    ShowError("Заполните все поля");
                    return;
                }

                if (Core.Context.Users.Any(x => x.Login == login))
                {
                    ShowError("Логин занят");
                    return;
                }

                var user = new Users { Login = login, PasswordHash = password, Email = login + "@mail.ru", DisplayName = login, RoleId = 1, IsFrozen = false, CreatedAt = DateTime.Now };
                Core.Context.Users.Add(user);
                Core.Context.SaveChanges();

                App.CurrentUser = user;
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.UpdateSidebar(true);
                mainWindow.MainFrame.Navigate(new CatalogPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка регистрации: " + ex.Message);
            }
        }

        private void Btn_UnfreezeRequest(object s, RoutedEventArgs e)
        {
            try
            {
                if (_frozenUser == null)
                {
                    ShowError("Сначала введите данные замороженного аккаунта и нажмите Войти");
                    return;
                }

                bool exists = Core.Context.UnfreezeRequests.Any(r => r.UserId == _frozenUser.UserId && !r.BookId.HasValue && !r.ReviewId.HasValue && r.Status == "Pending");
                if (exists)
                {
                    MessageBox.Show("Заявка уже ожидает рассмотрения");
                    return;
                }

                string reason = string.IsNullOrWhiteSpace(TbUnfreezeReason.Text) ? "Прошу разморозить аккаунт" : TbUnfreezeReason.Text.Trim();
                Core.Context.UnfreezeRequests.Add(new UnfreezeRequests { UserId = _frozenUser.UserId, Reason = reason, Status = "Pending", CreatedAt = DateTime.Now });
                Core.Context.SaveChanges();

                MessageBox.Show("Заявка на разморозку отправлена");
                UnfreezePanel.Visibility = Visibility.Collapsed;
                _frozenUser = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка отправки заявки: " + ex.Message);
            }
        }

        private void ShowError(string message)
        {
            TbError.Text = message;
            TbError.Visibility = Visibility.Visible;
        }
    }
}
