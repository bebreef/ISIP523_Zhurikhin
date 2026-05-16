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
            try
            {
                if (App.CurrentUser == null)
                {
                    MessageBox.Show("Ошибка: Вы не вошли в систему!", "Доступ запрещен",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    NavigateToCatalog();
                    return;
                }

                if (App.CurrentUser.RoleId != 3)
                {
                    MessageBox.Show("Ошибка: Доступ разрешен только администраторам!", "Доступ запрещен",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    NavigateToCatalog();
                    return;
                }

                InitializeComponent();
                LoadData();
            }
            catch (Exception ex)
            {
                NavigateToCatalog();
            }
        }

        private void NavigateToCatalog()
        {
            try
            {
                var mw = Application.Current.MainWindow as MainWindow;
                if (mw != null && mw.MainFrame != null)
                {
                    mw.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mw.MainFrame.Navigate(new CatalogPage());
                    }), System.Windows.Threading.DispatcherPriority.Background);
                }
            }
            catch { }
        }

        private void LoadData()
        {
            try
            {
                if (GridUsers != null)
                    GridUsers.ItemsSource = Core.Context.Users.Include("Roles").ToList();

                if (GridBooks != null)
                    GridBooks.ItemsSource = Core.Context.Books.Include("Users").ToList();

                if (GridReviews != null)
                {
                    GridReviews.ItemsSource = Core.Context.Reviews
                        .Include("Users")
                        .Include("Books")
                        .ToList();
                }

                if (GridComplaints != null)
                {
                    var complaints = Core.Context.Complaints.Include("Users").ToList();
                    GridComplaints.ItemsSource = complaints.Select(c => new
                    {
                        c.ComplaintId,
                        c.UserId,
                        UserName = c.Users != null && c.Users.DisplayName != null ? c.Users.DisplayName : "Неизвестно",
                        ComplaintType = c.BookId != null ? "Книга" : (c.ReviewId != null ? "Отзыв" : "Пользователь"),
                        ObjectId = c.BookId.HasValue ? (int?)c.BookId : (c.ReviewId.HasValue ? (int?)c.ReviewId : c.TargetUserId),
                        ObjectTitle = GetComplaintObjectTitle(c),
                        c.Reason,
                        c.CreatedAt
                    }).ToList();
                }

                if (GridRoleRequests != null)
                {
                    GridRoleRequests.ItemsSource = Core.Context.RoleRequests
                        .Include("Users")
                        .Where(r => r.Status == "Pending")
                        .ToList();
                }

                if (GridUnfreezeRequests != null)
                {
                    var unfreeze = Core.Context.UnfreezeRequests.Include("Users").ToList();
                    GridUnfreezeRequests.ItemsSource = unfreeze.Select(r => new
                    {
                        r.RequestId,
                        UserName = r.Users != null && r.Users.DisplayName != null ? r.Users.DisplayName : "Неизвестно",
                        RequestType = r.BookId != null ? "Книга" : (r.ReviewId.HasValue ? "Отзыв" : "Аккаунт"),
                        ObjectId = r.BookId.HasValue ? r.BookId.Value : (r.ReviewId.HasValue ? r.ReviewId.Value : 0),
                        r.Reason,
                        r.Status
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                string msg = "Ошибка загрузки данных: " + ex.Message;
                if (ex.InnerException != null)
                    msg += "\n\n" + ex.InnerException.Message;
                MessageBox.Show(msg, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private string GetComplaintObjectTitle(Complaints c)
        {
            try
            {
                if (c.BookId.HasValue)
                {
                    var b = Core.Context.Books.FirstOrDefault(x => x.BookId == c.BookId);
                    return b != null && b.Title != null ? b.Title : "Книга удалена";
                }
                if (c.ReviewId.HasValue)
                {
                    var r = Core.Context.Reviews.FirstOrDefault(x => x.ReviewId == c.ReviewId);
                    if (r != null && r.ReviewText != null)
                    {
                        return r.ReviewText.Length > 30
                            ? r.ReviewText.Substring(0, 30) + "..."
                            : r.ReviewText;
                    }
                    return "Отзыв";
                }
                if (c.TargetUserId.HasValue)
                {
                    var u = Core.Context.Users.FirstOrDefault(x => x.UserId == c.TargetUserId);
                    return u != null && u.DisplayName != null ? u.DisplayName : "Пользователь";
                }
                return "Неизвестно";
            }
            catch
            {
                return "Ошибка";
            }
        }

        private void Tab_Changed(object s, RoutedEventArgs e)
        {
            try
            {
                if (PanelUsers != null)
                    PanelUsers.Visibility = TabUsers.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;

                if (PanelBooks != null)
                    PanelBooks.Visibility = TabBooks.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;

                if (PanelReviews != null)
                    PanelReviews.Visibility = TabReviews.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;

                if (PanelComplaints != null)
                    PanelComplaints.Visibility = TabComplaints.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;

                if (PanelRoleRequests != null)
                    PanelRoleRequests.Visibility = TabRoleRequests.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;

                if (PanelUnfreezeRequests != null)
                    PanelUnfreezeRequests.Visibility = TabUnfreezeRequests.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;

                LoadData();
            }
            catch (Exception ex)
            {
                // Тихая обработка ошибки
            }
        }

        private void Btn_ToggleUserFreeze(object s, RoutedEventArgs e)
        {
            try
            {
                var btn = s as Button;
                if (btn != null && btn.Tag != null)
                {
                    int userId = (int)btn.Tag;
                    var user = Core.Context.Users.FirstOrDefault(u => u.UserId == userId);
                    if (user != null)
                    {
                        user.IsFrozen = !user.IsFrozen;
                        Core.Context.SaveChanges();
                        LoadData();
                        MessageBox.Show(user.IsFrozen ? "Пользователь заморожен" : "Пользователь разморожен");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }

        private void Btn_ResetUser(object s, RoutedEventArgs e)
        {
            try
            {
                var btn = s as Button;
                if (btn != null && btn.Tag != null)
                {
                    int userId = (int)btn.Tag;
                    var user = Core.Context.Users.FirstOrDefault(u => u.UserId == userId);
                    if (user != null && user.RoleId != 3)
                    {
                        user.RoleId = 1;
                        Core.Context.SaveChanges();
                        LoadData();
                        MessageBox.Show("Роль сброшена до Читателя");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }

        private void Btn_ToggleBookFreeze(object s, RoutedEventArgs e)
        {
            try
            {
                var btn = s as Button;
                if (btn != null && btn.Tag != null)
                {
                    int bookId = (int)btn.Tag;
                    var book = Core.Context.Books.FirstOrDefault(b => b.BookId == bookId);
                    if (book != null)
                    {
                        book.IsFrozen = !book.IsFrozen;
                        Core.Context.SaveChanges();
                        LoadData();
                        MessageBox.Show(book.IsFrozen ? "Книга заморожена" : "Книга разморожена");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }

        private void Btn_ToggleReviewFreeze(object s, RoutedEventArgs e)
        {
            try
            {
                var btn = s as Button;
                if (btn != null && btn.Tag != null)
                {
                    int reviewId = (int)btn.Tag;
                    var review = Core.Context.Reviews.FirstOrDefault(r => r.ReviewId == reviewId);
                    if (review != null)
                    {
                        review.IsFrozen = !review.IsFrozen;
                        Core.Context.SaveChanges();
                        LoadData();
                        MessageBox.Show(review.IsFrozen ? "Отзыв заморожен" : "Отзыв разморожен");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }

        private void Btn_DeleteComplaint(object s, RoutedEventArgs e)
        {
            try
            {
                var btn = s as Button;
                if (btn != null && btn.Tag != null)
                {
                    int complaintId = (int)btn.Tag;
                    var complaint = Core.Context.Complaints.FirstOrDefault(c => c.ComplaintId == complaintId);
                    if (complaint != null)
                    {
                        Core.Context.Complaints.Remove(complaint);
                        Core.Context.SaveChanges();
                        LoadData();
                        MessageBox.Show("Жалоба удалена");
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = "Ошибка: " + ex.Message;
                if (ex.InnerException != null) msg += "\n\n" + ex.InnerException.Message;
                MessageBox.Show(msg);
            }
        }

        private void Btn_FreezeObjectFromComplaint(object s, RoutedEventArgs e)
        {
            try
            {
                var btn = s as Button;
                if (btn != null && btn.Tag != null)
                {
                    int complaintId = (int)btn.Tag;
                    var complaint = Core.Context.Complaints.FirstOrDefault(c => c.ComplaintId == complaintId);
                    if (complaint == null) return;

                    if (complaint.BookId.HasValue)
                    {
                        var book = Core.Context.Books.FirstOrDefault(b => b.BookId == complaint.BookId);
                        if (book != null)
                        {
                            book.IsFrozen = true;
                            Core.Context.SaveChanges();
                            MessageBox.Show("Книга заморожена");
                        }
                    }
                    else if (complaint.ReviewId.HasValue)
                    {
                        var review = Core.Context.Reviews.FirstOrDefault(r => r.ReviewId == complaint.ReviewId);
                        if (review != null)
                        {
                            review.IsFrozen = true;
                            Core.Context.SaveChanges();
                            MessageBox.Show("Отзыв заморожен");
                        }
                    }
                    else if (complaint.TargetUserId.HasValue)
                    {
                        var user = Core.Context.Users.FirstOrDefault(u => u.UserId == complaint.TargetUserId);
                        if (user != null)
                        {
                            user.IsFrozen = true;
                            Core.Context.SaveChanges();
                            MessageBox.Show("Пользователь заморожен");
                        }
                    }
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                string msg = "Ошибка: " + ex.Message;
                if (ex.InnerException != null) msg += "\n\n" + ex.InnerException.Message;
                MessageBox.Show(msg);
            }
        }

        private void Btn_ApproveRole(object s, RoutedEventArgs e)
        {
            try
            {
                var btn = s as Button;
                if (btn != null && btn.Tag != null)
                {
                    int reqId = (int)btn.Tag;
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
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }

        private void Btn_RejectRole(object s, RoutedEventArgs e)
        {
            try
            {
                var btn = s as Button;
                if (btn != null && btn.Tag != null)
                {
                    int reqId = (int)btn.Tag;
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
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }

        private void Btn_ApproveUnfreeze(object s, RoutedEventArgs e)
        {
            try
            {
                var btn = s as Button;
                if (btn != null && btn.Tag != null)
                {
                    int reqId = (int)btn.Tag;
                    var req = Core.Context.UnfreezeRequests.FirstOrDefault(r => r.RequestId == reqId);
                    if (req != null)
                    {
                        req.Status = "Approved";
                        if (req.BookId.HasValue)
                        {
                            var book = Core.Context.Books.FirstOrDefault(b => b.BookId == req.BookId);
                            if (book != null) book.IsFrozen = false;
                        }
                        else if (req.ReviewId.HasValue)
                        {
                            var review = Core.Context.Reviews.FirstOrDefault(r => r.ReviewId == req.ReviewId);
                            if (review != null) review.IsFrozen = false;
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
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }

        private void Btn_RejectUnfreeze(object s, RoutedEventArgs e)
        {
            try
            {
                var btn = s as Button;
                if (btn != null && btn.Tag != null)
                {
                    int reqId = (int)btn.Tag;
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
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }
    }
}