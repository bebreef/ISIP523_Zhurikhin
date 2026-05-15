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

        public CatalogPage()
        {
            InitializeComponent();
            LoadGenres();
            LoadBooks();
        }

        private void LoadGenres()
        {
            try
            {
                var genres = Core.Context.Genres.ToList();
                CbGenre.ItemsSource = genres;
                CbGenre.SelectedIndex = -1; 
            }
            catch { }
        }

        private void LoadBooks()
        {
            try
            {
                var query = Core.Context.Books
                    .Include("Users")
                    .Include("Genres")
                    .Include("Reviews")
                    .AsQueryable();

                query = query.Where(b => b.IsFrozen == false);

                if (!string.IsNullOrWhiteSpace(TbTitle.Text))
                {
                    string search = TbTitle.Text.ToLower();
                    query = query.Where(b => b.Title != null && b.Title.ToLower().Contains(search));
                }

                if (!string.IsNullOrWhiteSpace(TbAuthor.Text))
                {
                    string author = TbAuthor.Text.ToLower();
                    query = query.Where(b => b.Users != null && b.Users.DisplayName != null &&
                                            b.Users.DisplayName.ToLower().Contains(author));
                }

                if (CbGenre.SelectedValue != null)
                {
                    int genreId = (int)CbGenre.SelectedValue;
                    query = query.Where(b => b.Genres != null && b.Genres.Any(g => g.GenreId == genreId));
                }

                var books = query.ToList();

                var sortItem = CbSort?.SelectedItem as ComboBoxItem;
                string sortValue = sortItem?.Content.ToString();

                if (sortValue == "По рейтингу")
                {
                    _allBooks = books.OrderByDescending(b =>
                        b.Reviews != null && b.Reviews.Any() ? b.Reviews.Average(r => r.Rating) : 0).ToList();
                }
                else
                {
                    _allBooks = books.OrderBy(b => b.Title ?? "").ToList();
                }

                BooksList.ItemsSource = _allBooks;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Каталог: {ex.Message}");
            }
        }

        private void TbTitle_TextChanged(object s, TextChangedEventArgs e) { LoadBooks(); }
        private void TbAuthor_TextChanged(object s, TextChangedEventArgs e) { LoadBooks(); }

        private void CbGenre_SelectionChanged(object s, SelectionChangedEventArgs e)
        {
            if (CbGenre.IsLoaded)
                LoadBooks();
        }

        private void CbSort_SelectionChanged(object s, SelectionChangedEventArgs e)
        {
            if (CbSort.IsLoaded)
                LoadBooks();
        }

        private void Btn_Read(object s, RoutedEventArgs e)
        {
            var btn = s as Button;
            var book = btn.DataContext as Books;
            if (book != null)
            {
                ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new BookPage(book));
            }
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