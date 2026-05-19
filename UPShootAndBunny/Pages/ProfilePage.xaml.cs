using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UPShootAndBunny.Pages
{
    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();

            if (App.CurrentUser == null)
            {
                ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new AuthPage());
                return;
            }

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var user = Core.Context.Users.Include("Roles").FirstOrDefault(u => u.UserId == App.CurrentUser.UserId);
                if (user == null) return;

                TbName.Text = user.DisplayName;
                TbLogin.Text = "Логин: " + user.Login;
                TbEmail.Text = "Email: " + user.Email;
                TbRole.Text = "Роль: " + (user.Roles != null ? user.Roles.RoleName : "Неизвестно");

                BtnRequestAuthor.Visibility = user.RoleId == 1 ? Visibility.Visible : Visibility.Collapsed;

                if (user.IsFrozen)
                {
                    var request = Core.Context.UnfreezeRequests.Where(r => r.UserId == user.UserId && !r.BookId.HasValue && !r.ReviewId.HasValue).OrderByDescending(r => r.CreatedAt).FirstOrDefault();
                    FreezeWarning.Visibility = Visibility.Visible;
                    TbFreezeReason.Text = "Аккаунт заморожен. Причина: " + (request != null ? request.Reason : "Неизвестно");
                    BtnDispute.Visibility = Visibility.Visible;
                }
                else
                {
                    FreezeWarning.Visibility = Visibility.Collapsed;
                    BtnDispute.Visibility = Visibility.Collapsed;
                }

                ReviewsList.ItemsSource = Core.Context.Reviews.Include("Books").Where(r => r.UserId == user.UserId).OrderByDescending(r => r.CreatedAt).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка профиля: " + ex.Message);
            }
        }

        private void ReviewCard_Click(object s, MouseButtonEventArgs e)
        {
            try
            {
                var border = s as FrameworkElement;
                if (border == null || border.Tag == null) return;

                int bookId = Convert.ToInt32(border.Tag);
                OpenBook(bookId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка открытия книги: " + ex.Message);
            }
        }

        private void OpenBook(int bookId)
        {
            var book = Core.Context.Books.Include("Users").Include("Genres").Include("Reviews").FirstOrDefault(b => b.BookId == bookId);
            if (book == null)
            {
                MessageBox.Show("Книга не найдена");
                return;
            }

            if (book.IsFrozen && App.CurrentUser.RoleId != 3)
            {
                MessageBox.Show("Книга временно недоступна");
                return;
            }

            ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new BookPage(book));
        }

        private void Btn_DisputeReview(object s, RoutedEventArgs e)
        {
            try
            {
                e.Handled = true;

                var button = s as Button;
                if (button == null || button.Tag == null) return;

                int reviewId = Convert.ToInt32(button.Tag);
                var review = Core.Context.Reviews.FirstOrDefault(r => r.ReviewId == reviewId && r.UserId == App.CurrentUser.UserId);
                if (review == null)
                {
                    MessageBox.Show("Отзыв не найден");
                    return;
                }

                if (!review.IsFrozen)
                {
                    MessageBox.Show("Этот отзыв не заморожен");
                    LoadData();
                    return;
                }

                bool exists = Core.Context.UnfreezeRequests.Any(r => r.UserId == App.CurrentUser.UserId && r.ReviewId == reviewId && r.Status == "Pending");
                if (exists)
                {
                    MessageBox.Show("Заявка на разморозку этого отзыва уже ожидает рассмотрения");
                    return;
                }

                Core.Context.UnfreezeRequests.Add(new UnfreezeRequests { UserId = App.CurrentUser.UserId, ReviewId = reviewId, Reason = "Прошу разморозить отзыв", Status = "Pending", CreatedAt = DateTime.Now });
                Core.Context.SaveChanges();
                MessageBox.Show("Заявка на разморозку отзыва отправлена");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка отправки заявки: " + ex.Message);
            }
        }

        private void Btn_RequestAuthor(object s, RoutedEventArgs e)
        {
            try
            {
                bool exists = Core.Context.RoleRequests.Any(r => r.UserId == App.CurrentUser.UserId && r.Status == "Pending");
                if (exists)
                {
                    MessageBox.Show("Заявка уже ожидает рассмотрения");
                    return;
                }

                Core.Context.RoleRequests.Add(new RoleRequests { UserId = App.CurrentUser.UserId, RequestedRoleId = 2, Reason = "Хочу публиковать книги", Status = "Pending", CreatedAt = DateTime.Now });
                Core.Context.SaveChanges();
                MessageBox.Show("Заявка отправлена");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка отправки заявки: " + ex.Message);
            }
        }

        private void Btn_Dispute(object s, RoutedEventArgs e)
        {
            try
            {
                bool exists = Core.Context.UnfreezeRequests.Any(r => r.UserId == App.CurrentUser.UserId && !r.BookId.HasValue && !r.ReviewId.HasValue && r.Status == "Pending");
                if (exists)
                {
                    MessageBox.Show("Заявка уже ожидает рассмотрения");
                    return;
                }

                Core.Context.UnfreezeRequests.Add(new UnfreezeRequests { UserId = App.CurrentUser.UserId, Reason = "Прошу разморозить аккаунт", Status = "Pending", CreatedAt = DateTime.Now });
                Core.Context.SaveChanges();
                MessageBox.Show("Заявка на разморозку отправлена");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка отправки заявки: " + ex.Message);
            }
        }
    }
}