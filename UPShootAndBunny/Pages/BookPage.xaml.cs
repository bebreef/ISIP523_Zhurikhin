using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UPShootAndBunny.Pages
{
    public partial class BookPage : Page
    {
        private Books _book;
        private Reviews _myReview;

        public BookPage(Books book)
        {
            InitializeComponent();

            if (book == null)
            {
                NavigateToCatalog();
                return;
            }

            _book = Core.Context.Books
                .Include("Users")
                .Include("Genres")
                .Include("Reviews")
                .FirstOrDefault(b => b.BookId == book.BookId);

            if (_book == null)
            {
                MessageBox.Show("Книга не найдена");
                NavigateToCatalog();
                return;
            }

            if ((_book.IsFrozen || (_book.Users != null && _book.Users.IsFrozen)) && (App.CurrentUser == null || App.CurrentUser.RoleId != 3))
            {
                MessageBox.Show("Эта книга временно недоступна");
                NavigateToCatalog();
                return;
            }

            LoadBookData();
        }

        private void LoadBookData()
        {
            DataContext = _book;

            TbAuthor.Text = _book.Users != null && !string.IsNullOrWhiteSpace(_book.Users.DisplayName) ? _book.Users.DisplayName : "Неизвестно";
            TbGenres.Text = _book.Genres != null ? string.Join(", ", _book.Genres.Select(g => g.GenreName)) : "";

            var activeReviews = Core.Context.Reviews.Where(r => r.BookId == _book.BookId && !r.IsFrozen).ToList();
            double rating = activeReviews.Any() ? activeReviews.Average(r => r.Rating) : 0;
            TbRating.Text = "Рейтинг: " + rating.ToString("F1") + "/10";

            bool isAdmin = App.CurrentUser != null && App.CurrentUser.RoleId == 3;
            var reviews = Core.Context.Reviews
                .Include("Users")
                .Where(r => r.BookId == _book.BookId && (!r.IsFrozen || isAdmin))
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            ReviewsList.ItemsSource = reviews;
            BtnFreeze.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;

            LoadMyReview();

            Dispatcher.BeginInvoke(new Action(UpdateReviewButtons), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void LoadMyReview()
        {
            if (App.CurrentUser == null)
            {
                ReviewPanel.Visibility = Visibility.Collapsed;
                return;
            }

            ReviewPanel.Visibility = Visibility.Visible;
            _myReview = Core.Context.Reviews.FirstOrDefault(r => r.BookId == _book.BookId && r.UserId == App.CurrentUser.UserId);

            if (_myReview == null)
            {
                TbReviewTitle.Text = "Ваш отзыв";
                TbReview.Text = "";
                SelectRating(10);
                BtnReview.Content = "Оставить отзыв";
                return;
            }

            TbReviewTitle.Text = "Редактирование вашего отзыва";
            TbReview.Text = _myReview.ReviewText;
            SelectRating(_myReview.Rating);
            BtnReview.Content = "Сохранить отзыв";
        }

        private void SelectRating(int rating)
        {
            foreach (var item in CbRating.Items)
            {
                var comboBoxItem = item as ComboBoxItem;
                if (comboBoxItem != null && comboBoxItem.Content != null && comboBoxItem.Content.ToString() == rating.ToString())
                {
                    CbRating.SelectedItem = comboBoxItem;
                    return;
                }
            }

            CbRating.SelectedIndex = 0;
        }

        private void UpdateReviewButtons()
        {
            bool isAdmin = App.CurrentUser != null && App.CurrentUser.RoleId == 3;

            for (int i = 0; i < ReviewsList.Items.Count; i++)
            {
                var container = ReviewsList.ItemContainerGenerator.ContainerFromItem(ReviewsList.Items[i]) as DependencyObject;
                if (container == null) continue;

                var complaintButton = FindChild<Button>(container, "BtnComplaintReview");
                var freezeButton = FindChild<Button>(container, "BtnFreezeReview");

                if (complaintButton != null) complaintButton.Visibility = isAdmin ? Visibility.Collapsed : Visibility.Visible;
                if (freezeButton != null) freezeButton.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private T FindChild<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            if (parent == null) return null;

            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                var element = child as FrameworkElement;

                if (child is T && element != null && element.Name == name) return (T)child;

                var result = FindChild<T>(child, name);
                if (result != null) return result;
            }

            return null;
        }

        private void Btn_Read(object s, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new ReadingPage(_book));
        }

        private void Btn_ComplaintBook(object s, RoutedEventArgs e)
        {
            AddComplaint(_book.BookId, null, null, "Жалоба на книгу");
        }

        private void Btn_ComplaintAuthor(object s, RoutedEventArgs e)
        {
            AddComplaint(null, null, _book.AuthorId, "Жалоба на автора");
        }

        private void Btn_ComplaintReview(object s, RoutedEventArgs e)
        {
            var button = s as Button;
            if (button == null || button.Tag == null) return;

            int reviewId = Convert.ToInt32(button.Tag);
            AddComplaint(null, reviewId, null, "Жалоба на отзыв");
        }

        private void AddComplaint(int? bookId, int? reviewId, int? targetUserId, string reason)
        {
            try
            {
                if (App.CurrentUser == null)
                {
                    MessageBox.Show("Сначала войдите в аккаунт");
                    return;
                }

                if (targetUserId.HasValue && targetUserId.Value == App.CurrentUser.UserId)
                {
                    MessageBox.Show("Нельзя пожаловаться на себя");
                    return;
                }

                bool exists = Core.Context.Complaints.Any(c => c.UserId == App.CurrentUser.UserId && c.BookId == bookId && c.ReviewId == reviewId && c.TargetUserId == targetUserId);
                if (exists)
                {
                    MessageBox.Show("Такая жалоба уже отправлена");
                    return;
                }

                Core.Context.Complaints.Add(new Complaints { UserId = App.CurrentUser.UserId, BookId = bookId, ReviewId = reviewId, TargetUserId = targetUserId, Reason = reason, CreatedAt = DateTime.Now });
                Core.Context.SaveChanges();
                MessageBox.Show("Жалоба отправлена");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка отправки жалобы: " + ex.Message + "\nЕсли это жалоба на автора, примените db_patch.sql");
            }
        }

        private void Btn_FreezeBook(object s, RoutedEventArgs e)
        {
            if (MessageBox.Show("Заморозить книгу?", "Админ", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

            _book.IsFrozen = true;
            Core.Context.SaveChanges();
            MessageBox.Show("Книга заморожена");
            LoadBookData();
        }

        private void Btn_FreezeReview(object s, RoutedEventArgs e)
        {
            var button = s as Button;
            if (button == null || button.Tag == null) return;

            int reviewId = Convert.ToInt32(button.Tag);
            var review = Core.Context.Reviews.FirstOrDefault(r => r.ReviewId == reviewId);
            if (review == null) return;

            review.IsFrozen = !review.IsFrozen;
            Core.Context.SaveChanges();
            MessageBox.Show(review.IsFrozen ? "Отзыв заморожен" : "Отзыв разморожен");
            LoadBookData();
        }

        private void Btn_AddReview(object s, RoutedEventArgs e)
        {
            try
            {
                if (App.CurrentUser == null)
                {
                    MessageBox.Show("Сначала войдите в аккаунт");
                    return;
                }

                if (string.IsNullOrWhiteSpace(TbReview.Text))
                {
                    MessageBox.Show("Введите текст отзыва");
                    return;
                }

                var item = CbRating.SelectedItem as ComboBoxItem;
                int rating;
                if (item == null || item.Content == null || !int.TryParse(item.Content.ToString(), out rating) || rating < 1 || rating > 10)
                {
                    MessageBox.Show("Выберите рейтинг от 1 до 10");
                    return;
                }

                var review = Core.Context.Reviews.FirstOrDefault(r => r.BookId == _book.BookId && r.UserId == App.CurrentUser.UserId);

                if (review == null)
                {
                    review = new Reviews { BookId = _book.BookId, UserId = App.CurrentUser.UserId, CreatedAt = DateTime.Now, IsFrozen = false };
                    Core.Context.Reviews.Add(review);
                }

                review.ReviewText = TbReview.Text.Trim();
                review.Rating = rating;
                review.CreatedAt = DateTime.Now;

                Core.Context.SaveChanges();
                MessageBox.Show(_myReview == null ? "Отзыв добавлен" : "Отзыв обновлен");
                LoadBookData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения отзыва: " + ex.Message);
            }
        }

        private void NavigateToCatalog()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null) mainWindow.MainFrame.Navigate(new CatalogPage());
        }
    }
}
