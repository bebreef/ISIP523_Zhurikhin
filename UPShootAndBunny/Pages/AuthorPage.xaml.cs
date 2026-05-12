using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UPShootAndBunny.Pages
{
    public partial class AuthorPage : Page
    {
        public AuthorPage()
        {
            InitializeComponent();
            LoadBooks();
        }

        private void LoadBooks()
        {
            try
            {
                var books = Core.Context.Books
                    .Where(b => b.AuthorId == App.CurrentUser.UserId)
                    .ToList();
                BooksList.ItemsSource = books;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки книг: {ex.Message}");
            }
        }

        private void Btn_AddBook(object s, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new AddBookPage());
        }

        private void Btn_Edit(object s, RoutedEventArgs e)
        {
            var btn = s as Button;
            var book = btn?.Tag as Books;
            if (book != null)
            {
                ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new AddBookPage(book));
            }
        }

        private void Btn_Dispute(object s, RoutedEventArgs e)
        {
            try
            {
                var btn = s as Button;
                var book = btn?.Tag as Books;
                if (book == null) return;

                bool exists = Core.Context.UnfreezeRequests
                    .Any(r => r.UserId == App.CurrentUser.UserId
                           && r.BookId == book.BookId
                           && r.Status == "Pending");

                if (!exists)
                {
                    Core.Context.UnfreezeRequests.Add(new UnfreezeRequests
                    {
                        UserId = App.CurrentUser.UserId,
                        BookId = book.BookId,
                        Reason = "Оспаривание заморозки книги",
                        Status = "Pending",
                        CreatedAt = DateTime.Now
                    });
                    Core.Context.SaveChanges();
                    MessageBox.Show("✓ Заявка на разморозку отправлена");
                    LoadBooks();
                }
                else
                {
                    MessageBox.Show("Заявка уже отправлена и ожидает рассмотрения");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}