using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UPShootAndBunny.Pages
{
    public partial class CatalogPage : Page
    {
        private bool _isReady;
        private List<Books> _allBooks = new List<Books>();

        public CatalogPage()
        {
            InitializeComponent();
            LoadGenres();
            _isReady = true;
            LoadBooks();
        }

        private void LoadGenres()
        {
            try
            {
                CbGenre.ItemsSource = Core.Context.Genres.OrderBy(g => g.GenreName).ToList();
                CbGenre.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки жанров: " + ex.Message);
            }
        }

        private void LoadBooks()
        {
            if (!_isReady || BooksList == null) return;

            try
            {
                var books = Core.Context.Books
                    .Include("Users")
                    .Include("Genres")
                    .Include("Reviews")
                    .Where(b => !b.IsFrozen)
                    .ToList();

                books = books.Where(b => b.Users == null || !b.Users.IsFrozen).ToList();

                if (!string.IsNullOrWhiteSpace(TbTitle.Text))
                {
                    string title = TbTitle.Text.Trim().ToLower();
                    books = books.Where(b => !string.IsNullOrWhiteSpace(b.Title) && b.Title.ToLower().Contains(title)).ToList();
                }

                if (!string.IsNullOrWhiteSpace(TbAuthor.Text))
                {
                    string author = TbAuthor.Text.Trim().ToLower();
                    books = books.Where(b => b.Users != null && !string.IsNullOrWhiteSpace(b.Users.DisplayName) && b.Users.DisplayName.ToLower().Contains(author)).ToList();
                }

                if (CbGenre.SelectedValue != null)
                {
                    int genreId = Convert.ToInt32(CbGenre.SelectedValue);
                    books = books.Where(b => b.Genres != null && b.Genres.Any(g => g.GenreId == genreId)).ToList();
                }

                string sortValue = "";
                var sortItem = CbSort.SelectedItem as ComboBoxItem;
                if (sortItem != null && sortItem.Content != null) sortValue = sortItem.Content.ToString();

                if (sortValue == "По рейтингу")
                {
                    _allBooks = books.OrderByDescending(GetRating).ThenBy(b => b.Title ?? "").ToList();
                }
                else
                {
                    _allBooks = books.OrderBy(b => b.Title ?? "").ToList();
                }

                BooksList.ItemsSource = _allBooks;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка каталога: " + ex.Message);
            }
        }

        private double GetRating(Books book)
        {
            if (book == null || book.Reviews == null) return 0;

            var reviews = book.Reviews.Where(r => !r.IsFrozen).ToList();
            if (!reviews.Any()) return 0;

            return reviews.Average(r => r.Rating);
        }

        private void TbTitle_TextChanged(object s, TextChangedEventArgs e)
        {
            LoadBooks();
        }

        private void TbAuthor_TextChanged(object s, TextChangedEventArgs e)
        {
            LoadBooks();
        }

        private void CbGenre_SelectionChanged(object s, SelectionChangedEventArgs e)
        {
            LoadBooks();
        }

        private void CbSort_SelectionChanged(object s, SelectionChangedEventArgs e)
        {
            LoadBooks();
        }

        private void Btn_ResetFilters(object s, RoutedEventArgs e)
        {
            TbTitle.Text = "";
            TbAuthor.Text = "";
            CbGenre.SelectedIndex = -1;
            CbSort.SelectedIndex = 0;
            LoadBooks();
        }

        private void Btn_Read(object s, RoutedEventArgs e)
        {
            var button = s as Button;
            var book = button != null ? button.DataContext as Books : null;
            if (book == null) return;

            ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new BookPage(book));
        }

        private void Btn_AddToList(object s, RoutedEventArgs e)
        {
            try
            {
                if (App.CurrentUser == null)
                {
                    MessageBox.Show("Сначала войдите в аккаунт");
                    return;
                }

                var button = s as Button;
                var book = button != null ? button.DataContext as Books : null;
                if (book == null) return;

                bool exists = Core.Context.ReadingLists.Any(r => r.UserId == App.CurrentUser.UserId && r.BookId == book.BookId);
                if (exists)
                {
                    MessageBox.Show("Книга уже есть в ваших списках");
                    return;
                }

                Core.Context.ReadingLists.Add(new ReadingLists { UserId = App.CurrentUser.UserId, BookId = book.BookId, Status = "В планах", AddedAt = DateTime.Now });
                Core.Context.SaveChanges();
                MessageBox.Show("Книга добавлена в список");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления в список: " + ex.Message);
            }
        }
    }
}
