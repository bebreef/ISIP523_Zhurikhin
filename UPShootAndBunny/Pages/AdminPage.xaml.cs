using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;

namespace UPShootAndBunny.Pages
{
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            if (!IsUserAuthorized())
                return;

            InitializeComponent();
            LoadData();
        }

        private bool IsUserAuthorized()
        {
            if (App.CurrentUser == null)
            {
                MessageBox.Show("Ошибка: Вы не вошли в систему!", "Доступ запрещен",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                GoBack();
                return false;
            }

            if (App.CurrentUser.RoleId != 3)
            {
                MessageBox.Show("Ошибка: Доступ разрешен только администраторам!", "Доступ запрещен",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                GoBack();
                return false;
            }

            return true;
        }

        private void GoBack()
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
            else
                ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new CatalogPage());
        }

        private void LoadData()
        {
            if (Core.Context == null)
            {
                MessageBox.Show("Ошибка подключения к базе данных", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var users = Core.Context.Users
                    .Include("Roles")         
                    .ToList();

                GridUsers.ItemsSource = users;

                var books = Core.Context.Books
                    .Include("Users")         
                    .ToList();

                GridBooks.ItemsSource = books;

                var complaints = Core.Context.Complaints
                    .Include("Users")          
                    .Include("Books")      
                    .Include("Reviews")         
                    .ToList();

                var complaintsData = complaints.Select(c => new
                {
                    c.ComplaintId,
                    UserName = c.Users?.DisplayName ?? "Неизвестно",
                    ComplaintType = c.BookId != null ? "Книга" : (c.ReviewId != null ? "Отзыв" : "Другое"),
                    ObjectTitle = GetComplaintObjectTitle(c),
                    c.Reason,
                    c.CreatedAt
                }).ToList();

                GridComplaints.ItemsSource = complaintsData;

                var roleRequests = Core.Context.RoleRequests
                    .Include("Users")        
                    .Where(r => r.Status == "Pending")
                    .ToList();

                GridRoleRequests.ItemsSource = roleRequests;

                var unfreeze = Core.Context.UnfreezeRequests
                    .Include("Users")          
                    .ToList();

                var unfreezeData = unfreeze.Select(r => new
                {
                    r.RequestId,
                    UserName = r.Users?.DisplayName ?? "Неизвестно",
                    RequestType = r.BookId != null ? "Книга" : "Аккаунт",
                    ObjectTitle = GetUnfreezeObjectTitle(r),
                    r.Reason,
                    r.Status
                }).ToList();

                GridUnfreezeRequests.ItemsSource = unfreezeData;
            }
            catch (Exception ex)
            {
                string errorMsg = $"Ошибка загрузки данных:\n\n{ex.Message}";
                if (ex.InnerException != null)
                    errorMsg += $"\n\nInner: {ex.InnerException.Message}";

                MessageBox.Show(errorMsg, "Ошибка загрузки", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetComplaintObjectTitle(Complaints c)
        {
            try
            {
                if (c.BookId.HasValue)
                {
                    var book = Core.Context.Books.FirstOrDefault(x => x.BookId == c.BookId);
                    return book?.Title ?? "Книга удалена";
                }
                if (c.ReviewId.HasValue)
                {
                    var review = Core.Context.Reviews.FirstOrDefault(x => x.ReviewId == c.ReviewId);
                    if (review?.ReviewText != null)
                    {
                        return review.ReviewText.Length > 30
                            ? review.ReviewText.Substring(0, 30) + "..."
                            : review.ReviewText;
                    }
                    return "Отзыв";
                }
                return "Пользователь";
            }
            catch
            {
                return "Ошибка";
            }
        }

        private string GetUnfreezeObjectTitle(UnfreezeRequests r)
        {
            try
            {
                if (r.BookId.HasValue)
                {
                    var book = Core.Context.Books.FirstOrDefault(x => x.BookId == r.BookId);
                    return book?.Title ?? "Книга удалена";
                }
                return r.Users?.DisplayName ?? "Пользователь удален";
            }
            catch
            {
                return "Ошибка";
            }
        }

        private void Tab_Changed(object sender, RoutedEventArgs e)
        {
            PanelUsers.Visibility = TabUsers.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            PanelBooks.Visibility = TabBooks.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            PanelComplaints.Visibility = TabComplaints.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            PanelRoleRequests.Visibility = TabRoleRequests.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            PanelUnfreezeRequests.Visibility = TabUnfreezeRequests.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;

            LoadData(); 
        }

        private void Btn_ToggleUserFreeze(object s, RoutedEventArgs e)
        {
            try
            {
                if (s is Button btn && btn.Tag is int userId)
                {
                    var user = Core.Context.Users.FirstOrDefault(u => u.UserId == userId);
                    if (user != null)
                    {
                        user.IsFrozen = !user.IsFrozen;
                        Core.Context.SaveChanges();
                        LoadData();
                        MessageBox.Show(user.IsFrozen ? "Пользователь заморожен" : "Пользователь разморожен");
                    }
                    else
                    {
                        MessageBox.Show("Пользователь не найден");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"Ошибка: {ex.Message}"); }
        }

        private void Btn_ResetUser(object s, RoutedEventArgs e)
        {
            try
            {
                if (s is Button btn && btn.Tag is int userId)
                {
                    var user = Core.Context.Users.FirstOrDefault(u => u.UserId == userId);
                    if (user != null && user.RoleId != 3)
                    {
                        user.RoleId = 1;
                        Core.Context.SaveChanges();
                        LoadData();
                        MessageBox.Show("Роль сброшена до Читателя");
                    }
                    else if (user?.RoleId == 3)
                    {
                        MessageBox.Show("Нельзя сбросить роль администратора");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"Ошибка: {ex.Message}"); }
        }

        private void Btn_ToggleBookFreeze(object s, RoutedEventArgs e)
        {
            try
            {
                if (s is Button btn && btn.Tag is int bookId)
                {
                    var book = Core.Context.Books.FirstOrDefault(b => b.BookId == bookId);
                    if (book != null)
                    {
                        book.IsFrozen = !book.IsFrozen;
                        Core.Context.SaveChanges();
                        LoadData();
                        MessageBox.Show(book.IsFrozen ? "Книга заморожена" : "Книга разморожена");
                    }
                    else
                    {
                        MessageBox.Show("Книга не найдена");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"Ошибка: {ex.Message}"); }
        }

        private void Btn_DeleteObject(object s, RoutedEventArgs e)
        {
            try
            {
                if (s is Button btn && btn.Tag is int complaintId)
                {
                    var comp = Core.Context.Complaints.FirstOrDefault(c => c.ComplaintId == complaintId);
                    if (comp == null)
                    {
                        MessageBox.Show("Жалоба не найдена");
                        return;
                    }

                    if (comp.BookId.HasValue)
                    {
                        var book = Core.Context.Books.FirstOrDefault(b => b.BookId == comp.BookId);
                        if (book != null) Core.Context.Books.Remove(book);
                    }
                    else if (comp.ReviewId.HasValue)
                    {
                        var review = Core.Context.Reviews.FirstOrDefault(r => r.ReviewId == comp.ReviewId);
                        if (review != null) Core.Context.Reviews.Remove(review);
                    }

                    Core.Context.Complaints.Remove(comp);
                    Core.Context.SaveChanges();
                    LoadData();
                    MessageBox.Show("Объект удален");
                }
            }
            catch (Exception ex) { MessageBox.Show($"Ошибка: {ex.Message}"); }
        }

        private void Btn_ApproveRole(object s, RoutedEventArgs e)
        {
            try
            {
                if (s is Button btn && btn.Tag is int reqId)
                {
                    var req = Core.Context.RoleRequests.FirstOrDefault(r => r.RequestId == reqId);
                    if (req != null)
                    {
                        req.Status = "Approved";
                        var user = Core.Context.Users.FirstOrDefault(u => u.UserId == req.UserId);
                        if (user != null) user.RoleId = 2;
                        Core.Context.SaveChanges();
                        LoadData();
                        MessageBox.Show("Заявка одобрена");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"Ошибка: {ex.Message}"); }
        }

        private void Btn_RejectRole(object s, RoutedEventArgs e)
        {
            try
            {
                if (s is Button btn && btn.Tag is int reqId)
                {
                    var req = Core.Context.RoleRequests.FirstOrDefault(r => r.RequestId == reqId);
                    if (req != null)
                    {
                        req.Status = "Rejected";
                        Core.Context.SaveChanges();
                        LoadData();
                        MessageBox.Show("Заявка отклонена");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"Ошибка: {ex.Message}"); }
        }

        private void Btn_ApproveUnfreeze(object s, RoutedEventArgs e)
        {
            try
            {
                if (s is Button btn && btn.Tag is int reqId)
                {
                    var req = Core.Context.UnfreezeRequests.FirstOrDefault(r => r.RequestId == reqId);
                    if (req != null)
                    {
                        req.Status = "Approved";
                        if (req.BookId.HasValue)
                        {
                            var book = Core.Context.Books.FirstOrDefault(b => b.BookId == req.BookId);
                            if (book != null) book.IsFrozen = false;
                        }
                        else
                        {
                            var user = Core.Context.Users.FirstOrDefault(u => u.UserId == req.UserId);
                            if (user != null) user.IsFrozen = false;
                        }
                        Core.Context.SaveChanges();
                        LoadData();
                        MessageBox.Show("Разморозка одобрена");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"Ошибка: {ex.Message}"); }
        }

        private void Btn_RejectUnfreeze(object s, RoutedEventArgs e)
        {
            try
            {
                if (s is Button btn && btn.Tag is int reqId)
                {
                    var req = Core.Context.UnfreezeRequests.FirstOrDefault(r => r.RequestId == reqId);
                    if (req != null)
                    {
                        req.Status = "Rejected";
                        Core.Context.SaveChanges();
                        LoadData();
                        MessageBox.Show("Заявка отклонена");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"Ошибка: {ex.Message}"); }
        }
    }
}