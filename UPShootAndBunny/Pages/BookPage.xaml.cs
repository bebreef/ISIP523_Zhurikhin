using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UPShootAndBunny.Pages
{
    public partial class BookPage : Page
    {
        private Books _book;
        public BookPage(Books book) { InitializeComponent(); _book = book; DataContext = _book; LoadData(); }
        private void LoadData()
        {
            TbAuthor.Text = _book.Users.DisplayName;
            var genreNames = _book.Genres.Select(g => g.GenreName).ToList();
            TbGenres.Text = string.Join(", ", genreNames);
            double avg = _book.Reviews.Any() ? _book.Reviews.Average(r => r.Rating) : 0;
            TbRating.Text = $"Рейтинг: {avg:F1}/10";
            ReviewsList.ItemsSource = Core.Context.Reviews.Include("Users").Where(r => r.BookId == _book.BookId).OrderByDescending(r => r.CreatedAt).ToList();
            if (App.CurrentUser.RoleId == 3) { BtnFreeze.Visibility = Visibility.Visible; }
        }
        private void Btn_Read(object s, RoutedEventArgs e) { var btn = s as Button; var book = btn.DataContext as Books; if (book != null) { ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new ReadingPage(book)); } }
        private void Btn_ComplaintBook(object s, RoutedEventArgs e) { string reason = "Жалоба на книгу"; Core.Context.Complaints.Add(new Complaints { UserId = App.CurrentUser.UserId, BookId = _book.BookId, Reason = reason, CreatedAt = DateTime.Now }); Core.Context.SaveChanges(); MessageBox.Show("Жалоба отправлена"); }
        private void Btn_ComplaintAuthor(object s, RoutedEventArgs e) { string reason = "Жалоба на автора"; Core.Context.Complaints.Add(new Complaints { UserId = App.CurrentUser.UserId, BookId = _book.BookId, Reason = reason, CreatedAt = DateTime.Now }); Core.Context.SaveChanges(); MessageBox.Show("Жалоба отправлена"); }
        private void Btn_FreezeBook(object s, RoutedEventArgs e) { if (MessageBox.Show("Заморозить книгу?", "Админ", MessageBoxButton.YesNo) == MessageBoxResult.Yes) { _book.IsFrozen = true; Core.Context.SaveChanges(); MessageBox.Show("Книга заморожена"); LoadData(); } }
        private void Btn_ComplaintReview(object s, RoutedEventArgs e) { var btn = s as Button; int reviewId = (int)btn.Tag; string reason = "Жалоба на отзыв"; Core.Context.Complaints.Add(new Complaints { UserId = App.CurrentUser.UserId, ReviewId = reviewId, Reason = reason, CreatedAt = DateTime.Now }); Core.Context.SaveChanges(); MessageBox.Show("Жалоба отправлена"); }
        private void Btn_AddReview(object s, RoutedEventArgs e) { if (string.IsNullOrWhiteSpace(TbReview.Text)) { MessageBox.Show("Введите текст"); return; } int rating = int.Parse(((ComboBoxItem)CbRating.SelectedItem).Content.ToString()); Core.Context.Reviews.Add(new Reviews { BookId = _book.BookId, UserId = App.CurrentUser.UserId, ReviewText = TbReview.Text, Rating = rating, CreatedAt = DateTime.Now }); Core.Context.SaveChanges(); MessageBox.Show("Отзыв добавлен"); LoadData(); TbReview.Clear(); }
    }
}