using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;

namespace UPShootAndBunny.Pages
{
    public partial class BookPage : Page
    {
        private Books _book;

        public BookPage(Books book)
        {
            InitializeComponent();
            _book = book;

            if (_book.IsFrozen && App.CurrentUser != null && App.CurrentUser.RoleId != 3)
            {
                MessageBox.Show("Эта книга временно недоступна", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                if (NavigationService != null && NavigationService.CanGoBack)
                    NavigationService.GoBack();
                return;
            }

            LoadBookData();
        }

        private void LoadBookData()
        {
            DataContext = _book;
            TbAuthor.Text = _book.Users != null && _book.Users.DisplayName != null ? _book.Users.DisplayName : "Неизвестно";

            var genres = _book.Genres != null ? _book.Genres.Select(g => g.GenreName).ToList() : new System.Collections.Generic.List<string>();
            TbGenres.Text = string.Join(", ", genres);

            double avg = 0;
            if (_book.Reviews != null && _book.Reviews.Any())
                avg = _book.Reviews.Average(r => r.Rating);
            TbRating.Text = "Рейтинг: " + avg.ToString("F1") + "/10";

            var reviews = Core.Context.Reviews
                .Include("Users")
                .Where(r => r.BookId == _book.BookId)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            ReviewsList.ItemsSource = reviews;

            if (App.CurrentUser != null && App.CurrentUser.RoleId == 3)
            {
                BtnFreeze.Visibility = Visibility.Visible;
            }
            else
            {
                BtnFreeze.Visibility = Visibility.Collapsed;
            }

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateReviewButtons();
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void UpdateReviewButtons()
        {
            for (int i = 0; i < ReviewsList.Items.Count; i++)
            {
                var container = ReviewsList.ItemContainerGenerator.ContainerFromItem(ReviewsList.Items[i]) as FrameworkElement;
                if (container != null)
                {
                    var complaintBtn = FindChild<Button>(container, "BtnComplaintReview");
                    var freezeBtn = FindChild<Button>(container, "BtnFreezeReview");

                    if (App.CurrentUser != null && App.CurrentUser.RoleId == 3)
                    {
                        if (complaintBtn != null) complaintBtn.Visibility = Visibility.Collapsed;
                        if (freezeBtn != null) freezeBtn.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        if (complaintBtn != null) complaintBtn.Visibility = Visibility.Visible;
                        if (freezeBtn != null) freezeBtn.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);

                if (child is T typedChild && (child as FrameworkElement).Name == childName)
                    return typedChild;

                var childOfChild = FindChild<T>(child, childName);
                if (childOfChild != null)
                    return childOfChild;
            }

            return null;
        }

        private void Btn_Read(object s, RoutedEventArgs e)
        {
            var btn = s as Button;
            var book = btn != null ? btn.DataContext as Books : null;
            if (book != null)
            {
                var mw = Application.Current.MainWindow as MainWindow;
                if (mw != null)
                    mw.MainFrame.Navigate(new ReadingPage(book));
            }
        }

        private void Btn_ComplaintBook(object s, RoutedEventArgs e)
        {
            Core.Context.Complaints.Add(new Complaints
            {
                UserId = App.CurrentUser.UserId,
                BookId = _book.BookId,
                Reason = "Жалоба на книгу",
                CreatedAt = DateTime.Now
            });
            Core.Context.SaveChanges();
            MessageBox.Show("Жалоба отправлена");
        }

        private void Btn_ComplaintAuthor(object s, RoutedEventArgs e)
        {
            Core.Context.Complaints.Add(new Complaints
            {
                UserId = App.CurrentUser.UserId,
                TargetUserId = _book.AuthorId,
                Reason = "Жалоба на автора",
                CreatedAt = DateTime.Now
            });
            Core.Context.SaveChanges();
            MessageBox.Show("Жалоба отправлена");
        }

        private void Btn_FreezeBook(object s, RoutedEventArgs e)
        {
            if (MessageBox.Show("Заморозить книгу?", "Админ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _book.IsFrozen = true;
                Core.Context.SaveChanges();
                MessageBox.Show("Книга заморожена");
                LoadBookData();
            }
        }

        private void Btn_ComplaintReview(object s, RoutedEventArgs e)
        {
            var btn = s as Button;
            if (btn != null && btn.Tag != null)
            {
                int reviewId = (int)btn.Tag;
                Core.Context.Complaints.Add(new Complaints
                {
                    UserId = App.CurrentUser.UserId,
                    ReviewId = reviewId,
                    Reason = "Жалоба на отзыв",
                    CreatedAt = DateTime.Now
                });
                Core.Context.SaveChanges();
                MessageBox.Show("Жалоба отправлена");
            }
        }

        private void Btn_FreezeReview(object s, RoutedEventArgs e)
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
                    MessageBox.Show(review.IsFrozen ? "Отзыв заморожен" : "Отзыв разморожен");
                    LoadBookData();
                }
            }
        }

        private void Btn_AddReview(object s, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbReview.Text))
            {
                MessageBox.Show("Введите текст отзыва");
                return;
            }

            ComboBoxItem item = CbRating.SelectedItem as ComboBoxItem;
            if (item == null)
            {
                MessageBox.Show("Выберите рейтинг");
                return;
            }

            int rating = 0;
            int.TryParse(item.Content.ToString(), out rating);

            Core.Context.Reviews.Add(new Reviews
            {
                BookId = _book.BookId,
                UserId = App.CurrentUser.UserId,
                ReviewText = TbReview.Text,
                Rating = rating,
                CreatedAt = DateTime.Now
            });
            Core.Context.SaveChanges();
            MessageBox.Show("Отзыв добавлен");
            TbReview.Clear();
            LoadBookData();
        }
    }
}