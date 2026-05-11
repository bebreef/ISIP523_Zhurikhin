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
        private List<Books> _allBooks = new List<Books>();
        public CatalogPage() { InitializeComponent(); LoadGenres(); LoadBooks(); }

        private void LoadGenres()
        {
            try { CbGenre.ItemsSource = Core.Context.Genres.ToList(); }
            catch { }
        }

        private void LoadBooks()
        {
            try
            {
                var allBooks = Core.Context.Books
                    .Include("Users")
                    .Include("Genres")
                    .Include("Reviews")
                    .ToList();

                var filteredBooks = allBooks.AsQueryable();

                if (!string.IsNullOrWhiteSpace(TbTitle.Text))
                {
                    filteredBooks = filteredBooks.Where(b => b.Title != null && b.Title.ToLower().Contains(TbTitle.Text.ToLower()));
                }

                if (!string.IsNullOrWhiteSpace(TbAuthor.Text))
                {
                    filteredBooks = filteredBooks.Where(b => b.Users != null && b.Users.DisplayName != null && b.Users.DisplayName.ToLower().Contains(TbAuthor.Text.ToLower()));
                }

                if (CbGenre.SelectedValue != null)
                {
                    int genreId = (int)CbGenre.SelectedValue;
                    filteredBooks = filteredBooks.Where(b => b.Genres != null && b.Genres.Any(g => g.GenreId == genreId));
                }

                var books = filteredBooks.ToList();

                var sortItem = CbSort.SelectedItem as ComboBoxItem;
                string sortValue = sortItem?.Content.ToString();

                if (sortValue == "По рейтингу")
                {
                    _allBooks = books.OrderByDescending(b =>
                    {
                        if (b.Reviews == null || !b.Reviews.Any()) return 0;
                        return b.Reviews.Average(r => r.Rating);
                    }).ToList();
                }
                else
                {
                    _allBooks = books.OrderBy(b => b.Title ?? "").ToList();
                }

                BooksList.ItemsSource = _allBooks;
            }
            catch (NullReferenceException) { }
            catch (Exception ex)
            {
                MessageBox.Show($"Каталог: {ex.Message}");
            }
        }

        private void TbTitle_TextChanged(object s, TextChangedEventArgs e) { LoadBooks(); }
        private void TbAuthor_TextChanged(object s, TextChangedEventArgs e) { LoadBooks(); }
        private void CbGenre_SelectionChanged(object s, SelectionChangedEventArgs e) { LoadBooks(); }
        private void CbSort_SelectionChanged(object s, SelectionChangedEventArgs e) { LoadBooks(); }

        private void Btn_Read(object s, RoutedEventArgs e)
        {
            var btn = s as Button;
            var book = btn.DataContext as Books;
            if (book != null) { ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new BookPage(book)); }
        }

        private void Btn_AddToList(object s, RoutedEventArgs e)
        {
            var btn = s as Button;
            var book = btn.DataContext as Books;
            if (book != null)
            {
                bool exists = Core.Context.ReadingLists.Any(r => r.UserId == App.CurrentUser.UserId && r.BookId == book.BookId);
                if (!exists)
                {
                    Core.Context.ReadingLists.Add(new ReadingLists { UserId = App.CurrentUser.UserId, BookId = book.BookId, Status = "В планах", AddedAt = DateTime.Now });
                    Core.Context.SaveChanges();
                    MessageBox.Show("Книга добавлена в список 'В планах'");
                }
                else { MessageBox.Show("Книга уже есть в ваших списках"); }
            }
        }
    }
}