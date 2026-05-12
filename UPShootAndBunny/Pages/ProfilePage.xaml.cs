using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UPShootAndBunny.Pages
{
    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var user = Core.Context.Users.FirstOrDefault(u => u.UserId == App.CurrentUser.UserId);
                if (user == null) return;

                TbName.Text = user.DisplayName;
                TbLogin.Text = $"Логин: {user.Login}";
                TbEmail.Text = $"Email: {user.Email}";
                TbRole.Text = $"Роль: {user.Roles.RoleName}";

                if (App.CurrentUser.RoleId == 2) { BtnRequestAuthor.Visibility = Visibility.Collapsed; }
                else { BtnRequestAuthor.Visibility = Visibility.Visible; }

                if (user.IsFrozen)
                {
                    FreezeWarning.Visibility = Visibility.Visible;
                    TbFreezeReason.Text = "Аккаунт заморожен. Причина: " + Core.Context.UnfreezeRequests.FirstOrDefault(r => r.UserId == user.UserId)?.Reason ?? "Неизвестно";
                    BtnDispute.Visibility = Visibility.Visible;
                }
                else
                {
                    FreezeWarning.Visibility = Visibility.Collapsed;
                    BtnDispute.Visibility = Visibility.Collapsed;
                }

                ReviewsList.ItemsSource = Core.Context.Reviews.Where(r => r.UserId == user.UserId).OrderByDescending(r => r.CreatedAt).ToList();
            }
            catch (Exception ex) { MessageBox.Show($"Профиль: {ex.Message}"); }
        }

        private void Btn_RequestAuthor(object s, RoutedEventArgs e)
        {
            bool exists = Core.Context.RoleRequests.Any(r => r.UserId == App.CurrentUser.UserId && r.Status == "Pending");
            if (exists) { MessageBox.Show("Заявка уже ожидает рассмотрения."); return; }
            var req = new RoleRequests { UserId = App.CurrentUser.UserId, RequestedRoleId = 2, Reason = "Хочу публиковать книги", Status = "Pending", CreatedAt = DateTime.Now };
            Core.Context.RoleRequests.Add(req);
            Core.Context.SaveChanges();
            MessageBox.Show("Заявка отправлена!");
            LoadData();
        }

        private void Btn_Dispute(object s, RoutedEventArgs e)
        {
            MessageBox.Show("Заявка на разморозку отправлена.");
        }
    }
}