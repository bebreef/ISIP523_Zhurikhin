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
            if (App.CurrentUser == null)
            {
                MessageBox.Show("Ошибка: Вы не вошли в систему!", "Доступ запрещен",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                Application.Current.Dispatcher.BeginInvoke(new Action(() => NavigateToCatalog()));
                return;
            }

            if (App.CurrentUser.RoleId != 2)
            {
                MessageBox.Show("Ошибка: Доступ разрешен только авторам!", "Доступ запрещен",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                Application.Current.Dispatcher.BeginInvoke(new Action(() => NavigateToCatalog()));
                return;
            }

            InitializeComponent();
            LoadBooks();
        }

        private void NavigateToCatalog()
        {
            var mw = Application.Current.MainWindow as MainWindow;
            if (mw != null && mw.MainFrame != null)
            {
                mw.MainFrame.Navigate(new CatalogPage());
            }
        }

        private void LoadBooks()
        {
            try
            {
                var books = Core.Context.Books.Where(b => b.AuthorId == App.CurrentUser.UserId).ToList();
                BooksList.ItemsSource = books;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки книг: " + ex.Message);
            }
        }

        private void Btn_AddBook(object s, RoutedEventArgs e)
        {
            var mw = Application.Current.MainWindow as MainWindow;
            if (mw != null)
                mw.MainFrame.Navigate(new AddBookPage());
        }

        private void Btn_Edit(object s, RoutedEventArgs e)
        {
            var btn = s as Button;
            var book = btn != null ? btn.Tag as Books : null;
            if (book != null)
            {
                var mw = Application.Current.MainWindow as MainWindow;
                if (mw != null)
                    mw.MainFrame.Navigate(new AddBookPage(book));
            }
        }

        private void Btn_Dispute(object s, RoutedEventArgs e)
        {
            var btn = s as Button;
            var book = btn != null ? btn.Tag as Books : null;
            if (book == null) return;

            bool exists = Core.Context.UnfreezeRequests.Any(r => r.UserId == App.CurrentUser.UserId && r.BookId == book.BookId && r.Status == "Pending");
            if (!exists)
            {
                Core.Context.UnfreezeRequests.Add(new UnfreezeRequests { UserId = App.CurrentUser.UserId, BookId = book.BookId, Reason = "Оспаривание заморозки книги", Status = "Pending", CreatedAt = DateTime.Now });
                Core.Context.SaveChanges();
                MessageBox.Show("Заявка отправлена");
            }
            else
            {
                MessageBox.Show("Заявка уже отправлена");
            }
        }
    }
}