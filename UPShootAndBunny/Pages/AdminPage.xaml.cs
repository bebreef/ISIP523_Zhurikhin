using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UPShootAndBunny.Pages
{
    public partial class AdminPage : Page
    {
        private List<Roles> _roles = new List<Roles>();
        private Users _selectedUser;

        public AdminPage()
        {
            InitializeComponent();

            if (App.CurrentUser == null || App.CurrentUser.RoleId != 3)
            {
                MessageBox.Show("Доступ разрешен только администраторам");
                NavigateToCatalog();
                return;
            }

            LoadRoles();
            LoadData();
        }

        private void NavigateToCatalog()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null) mainWindow.MainFrame.Navigate(new CatalogPage());
        }

        private void LoadRoles()
        {
            _roles = Core.Context.Roles.OrderBy(r => r.RoleId).ToList();
            CbUserRole.ItemsSource = _roles;
        }

        private void LoadData()
        {
            try
            {
                GridUsers.ItemsSource = Core.Context.Users.Include("Roles").OrderBy(u => u.UserId).ToList();
                GridBooks.ItemsSource = Core.Context.Books.Include("Users").OrderByDescending(b => b.CreatedAt).ToList();
                GridReviews.ItemsSource = Core.Context.Reviews.Include("Users").Include("Books").OrderByDescending(r => r.CreatedAt).ToList();

                var complaints = Core.Context.Complaints.Include("Users").OrderByDescending(c => c.CreatedAt).ToList();
                GridComplaints.ItemsSource = complaints.Select(c => new
                {
                    c.ComplaintId,
                    UserName = c.Users != null && !string.IsNullOrWhiteSpace(c.Users.DisplayName) ? c.Users.DisplayName : "Неизвестно",
                    ComplaintType = c.BookId.HasValue ? "Книга" : (c.ReviewId.HasValue ? "Отзыв" : "Автор"),
                    ObjectTitle = GetComplaintObjectTitle(c),
                    c.Reason,
                    c.CreatedAt
                }).ToList();

                GridRoleRequests.ItemsSource = Core.Context.RoleRequests.Include("Users").Where(r => r.Status == "Pending").OrderBy(r => r.CreatedAt).ToList();

                var unfreezeRequests = Core.Context.UnfreezeRequests.Include("Users").Where(r => r.Status == "Pending").OrderBy(r => r.CreatedAt).ToList();
                GridUnfreezeRequests.ItemsSource = unfreezeRequests.Select(r => new
                {
                    r.RequestId,
                    UserName = r.Users != null && !string.IsNullOrWhiteSpace(r.Users.DisplayName) ? r.Users.DisplayName : "Неизвестно",
                    RequestType = r.BookId.HasValue ? "Книга" : (r.ReviewId.HasValue ? "Отзыв" : "Аккаунт"),
                    ObjectTitle = GetUnfreezeObjectTitle(r),
                    r.Reason,
                    r.Status
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }

        private string GetComplaintObjectTitle(Complaints complaint)
        {
            if (complaint == null) return "Неизвестно";

            if (complaint.BookId.HasValue)
            {
                var book = Core.Context.Books.FirstOrDefault(b => b.BookId == complaint.BookId.Value);
                return book != null && !string.IsNullOrWhiteSpace(book.Title) ? book.Title : "Книга удалена";
            }

            if (complaint.ReviewId.HasValue)
            {
                var review = Core.Context.Reviews.FirstOrDefault(r => r.ReviewId == complaint.ReviewId.Value);
                if (review == null) return "Отзыв удален";
                if (string.IsNullOrWhiteSpace(review.ReviewText)) return "Отзыв";
                return review.ReviewText.Length > 40 ? review.ReviewText.Substring(0, 40) + "..." : review.ReviewText;
            }

            if (complaint.TargetUserId.HasValue)
            {
                var user = Core.Context.Users.FirstOrDefault(u => u.UserId == complaint.TargetUserId.Value);
                return user != null && !string.IsNullOrWhiteSpace(user.DisplayName) ? user.DisplayName : "Пользователь удален";
            }

            return "Неизвестно";
        }

        private string GetUnfreezeObjectTitle(UnfreezeRequests request)
        {
            if (request == null) return "Неизвестно";

            if (request.BookId.HasValue)
            {
                var book = Core.Context.Books.FirstOrDefault(b => b.BookId == request.BookId.Value);
                return book != null && !string.IsNullOrWhiteSpace(book.Title) ? book.Title : "Книга";
            }

            if (request.ReviewId.HasValue)
            {
                var review = Core.Context.Reviews.FirstOrDefault(r => r.ReviewId == request.ReviewId.Value);
                if (review == null) return "Отзыв";
                if (string.IsNullOrWhiteSpace(review.ReviewText)) return "Отзыв";
                return review.ReviewText.Length > 40 ? review.ReviewText.Substring(0, 40) + "..." : review.ReviewText;
            }

            return "Аккаунт";
        }

        private void Tab_Changed(object s, RoutedEventArgs e)
        {
            if (PanelUsers == null) return;

            PanelUsers.Visibility = TabUsers.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            GridBooks.Visibility = TabBooks.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            GridReviews.Visibility = TabReviews.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            GridComplaints.Visibility = TabComplaints.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            GridRoleRequests.Visibility = TabRoleRequests.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            GridUnfreezeRequests.Visibility = TabUnfreezeRequests.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;

            LoadData();
        }

        private void GridUsers_SelectionChanged(object s, SelectionChangedEventArgs e)
        {
            _selectedUser = GridUsers.SelectedItem as Users;

            if (_selectedUser == null)
            {
                TbSelectedUser.Text = "Выберите пользователя в таблице";
                CbUserRole.SelectedIndex = -1;
                PbNewPassword.Password = "";
                return;
            }

            TbSelectedUser.Text = _selectedUser.DisplayName + " (" + _selectedUser.Login + ")";
            CbUserRole.SelectedValue = _selectedUser.RoleId;
            PbNewPassword.Password = "";
        }

        private void Btn_SaveUserChanges(object s, RoutedEventArgs e)
        {
            try
            {
                if (_selectedUser == null)
                {
                    MessageBox.Show("Выберите пользователя");
                    return;
                }

                if (CbUserRole.SelectedValue == null)
                {
                    MessageBox.Show("Выберите роль");
                    return;
                }

                int roleId = Convert.ToInt32(CbUserRole.SelectedValue);
                var user = Core.Context.Users.FirstOrDefault(u => u.UserId == _selectedUser.UserId);
                if (user == null) return;

                if (user.UserId == App.CurrentUser.UserId && roleId != 3)
                {
                    MessageBox.Show("Нельзя снять роль администратора с самого себя");
                    return;
                }

                user.RoleId = roleId;

                if (!string.IsNullOrWhiteSpace(PbNewPassword.Password))
                {
                    user.PasswordHash = PbNewPassword.Password;
                }

                Core.Context.SaveChanges();
                MessageBox.Show("Пользователь обновлен");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения пользователя: " + ex.Message);
            }
        }

        private void Btn_ToggleUserFreeze(object s, RoutedEventArgs e)
        {
            try
            {
                var button = s as Button;
                if (button == null || button.Tag == null) return;

                int userId = Convert.ToInt32(button.Tag);
                if (userId == App.CurrentUser.UserId)
                {
                    MessageBox.Show("Администратор не может заморозить сам себя");
                    return;
                }

                var user = Core.Context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null) return;

                user.IsFrozen = !user.IsFrozen;
                Core.Context.SaveChanges();
                LoadData();
                MessageBox.Show(user.IsFrozen ? "Пользователь заморожен" : "Пользователь разморожен");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка изменения статуса пользователя: " + ex.Message);
            }
        }

        private void Btn_ResetUser(object s, RoutedEventArgs e)
        {
            try
            {
                var button = s as Button;
                if (button == null || button.Tag == null) return;

                int userId = Convert.ToInt32(button.Tag);
                if (userId == App.CurrentUser.UserId)
                {
                    MessageBox.Show("Нельзя сбросить роль самому себе");
                    return;
                }

                var user = Core.Context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null) return;

                user.RoleId = 1;
                Core.Context.SaveChanges();
                LoadData();
                MessageBox.Show("Роль сброшена до читателя");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сброса роли: " + ex.Message);
            }
        }

        private void Btn_ToggleBookFreeze(object s, RoutedEventArgs e)
        {
            try
            {
                var button = s as Button;
                if (button == null || button.Tag == null) return;

                int bookId = Convert.ToInt32(button.Tag);
                var book = Core.Context.Books.FirstOrDefault(b => b.BookId == bookId);
                if (book == null) return;

                book.IsFrozen = !book.IsFrozen;
                Core.Context.SaveChanges();
                LoadData();
                MessageBox.Show(book.IsFrozen ? "Книга заморожена" : "Книга разморожена");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка изменения статуса книги: " + ex.Message);
            }
        }

        private void Btn_ToggleReviewFreeze(object s, RoutedEventArgs e)
        {
            try
            {
                var button = s as Button;
                if (button == null || button.Tag == null) return;

                int reviewId = Convert.ToInt32(button.Tag);
                var review = Core.Context.Reviews.FirstOrDefault(r => r.ReviewId == reviewId);
                if (review == null) return;

                review.IsFrozen = !review.IsFrozen;
                Core.Context.SaveChanges();
                LoadData();
                MessageBox.Show(review.IsFrozen ? "Отзыв заморожен" : "Отзыв разморожен");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка изменения статуса отзыва: " + ex.Message);
            }
        }

        private void Btn_DeleteComplaint(object s, RoutedEventArgs e)
        {
            try
            {
                var button = s as Button;
                if (button == null || button.Tag == null) return;

                int complaintId = Convert.ToInt32(button.Tag);
                var complaint = Core.Context.Complaints.FirstOrDefault(c => c.ComplaintId == complaintId);
                if (complaint == null) return;

                Core.Context.Complaints.Remove(complaint);
                Core.Context.SaveChanges();
                LoadData();
                MessageBox.Show("Жалоба удалена");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления жалобы: " + ex.Message);
            }
        }

        private void Btn_FreezeObjectFromComplaint(object s, RoutedEventArgs e)
        {
            try
            {
                var button = s as Button;
                if (button == null || button.Tag == null) return;

                int complaintId = Convert.ToInt32(button.Tag);
                var complaint = Core.Context.Complaints.FirstOrDefault(c => c.ComplaintId == complaintId);
                if (complaint == null) return;

                if (complaint.BookId.HasValue)
                {
                    var book = Core.Context.Books.FirstOrDefault(b => b.BookId == complaint.BookId.Value);
                    if (book != null) book.IsFrozen = true;
                }
                else if (complaint.ReviewId.HasValue)
                {
                    var review = Core.Context.Reviews.FirstOrDefault(r => r.ReviewId == complaint.ReviewId.Value);
                    if (review != null) review.IsFrozen = true;
                }
                else if (complaint.TargetUserId.HasValue)
                {
                    if (complaint.TargetUserId.Value == App.CurrentUser.UserId)
                    {
                        MessageBox.Show("Администратор не может заморозить сам себя");
                        return;
                    }

                    var user = Core.Context.Users.FirstOrDefault(u => u.UserId == complaint.TargetUserId.Value);
                    if (user != null) user.IsFrozen = true;
                }

                Core.Context.SaveChanges();
                LoadData();
                MessageBox.Show("Объект заморожен");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка заморозки объекта: " + ex.Message);
            }
        }

        private void Btn_ApproveRole(object s, RoutedEventArgs e)
        {
            try
            {
                var button = s as Button;
                if (button == null || button.Tag == null) return;

                int requestId = Convert.ToInt32(button.Tag);
                var request = Core.Context.RoleRequests.FirstOrDefault(r => r.RequestId == requestId);
                if (request == null) return;

                var user = Core.Context.Users.FirstOrDefault(u => u.UserId == request.UserId);
                if (user != null) user.RoleId = request.RequestedRoleId;

                request.Status = "Approved";
                Core.Context.SaveChanges();
                LoadData();
                MessageBox.Show("Заявка одобрена");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обработки заявки: " + ex.Message);
            }
        }

        private void Btn_RejectRole(object s, RoutedEventArgs e)
        {
            try
            {
                var button = s as Button;
                if (button == null || button.Tag == null) return;

                int requestId = Convert.ToInt32(button.Tag);
                var request = Core.Context.RoleRequests.FirstOrDefault(r => r.RequestId == requestId);
                if (request == null) return;

                request.Status = "Rejected";
                Core.Context.SaveChanges();
                LoadData();
                MessageBox.Show("Заявка отклонена");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обработки заявки: " + ex.Message);
            }
        }

        private void Btn_ApproveUnfreeze(object s, RoutedEventArgs e)
        {
            try
            {
                var button = s as Button;
                if (button == null || button.Tag == null) return;

                int requestId = Convert.ToInt32(button.Tag);
                var request = Core.Context.UnfreezeRequests.FirstOrDefault(r => r.RequestId == requestId);
                if (request == null) return;

                if (request.BookId.HasValue)
                {
                    var book = Core.Context.Books.FirstOrDefault(b => b.BookId == request.BookId.Value);
                    if (book != null) book.IsFrozen = false;
                }
                else if (request.ReviewId.HasValue)
                {
                    var review = Core.Context.Reviews.FirstOrDefault(r => r.ReviewId == request.ReviewId.Value);
                    if (review != null) review.IsFrozen = false;
                }
                else
                {
                    var user = Core.Context.Users.FirstOrDefault(u => u.UserId == request.UserId);
                    if (user != null) user.IsFrozen = false;
                }

                request.Status = "Approved";
                Core.Context.SaveChanges();
                LoadData();
                MessageBox.Show("Заявка на разморозку одобрена");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обработки заявки: " + ex.Message);
            }
        }

        private void Btn_RejectUnfreeze(object s, RoutedEventArgs e)
        {
            try
            {
                var button = s as Button;
                if (button == null || button.Tag == null) return;

                int requestId = Convert.ToInt32(button.Tag);
                var request = Core.Context.UnfreezeRequests.FirstOrDefault(r => r.RequestId == requestId);
                if (request == null) return;

                request.Status = "Rejected";
                Core.Context.SaveChanges();
                LoadData();
                MessageBox.Show("Заявка отклонена");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обработки заявки: " + ex.Message);
            }
        }
    }
}