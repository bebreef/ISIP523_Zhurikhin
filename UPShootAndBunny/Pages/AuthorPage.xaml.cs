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

            if (App.CurrentUser == null)
            {
                MessageBox.Show("Вы не вошли в систему");
                NavigateToCatalog();
                return;
            }

            if (App.CurrentUser.RoleId != 2)
            {
                MessageBox.Show("Доступ разрешен только авторам");
                NavigateToCatalog();
                return;
            }

            LoadBooks();
        }

        private void NavigateToCatalog()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null) mainWindow.MainFrame.Navigate(new CatalogPage());
        }

        private void LoadBooks()
        {
            try
            {
                BooksList.ItemsSource = Core.Context.Books.Where(b => b.AuthorId == App.CurrentUser.UserId).OrderByDescending(b => b.CreatedAt).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки книг: " + ex.Message);
            }
        }

        private void Btn_AddBook(object s, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null) mainWindow.MainFrame.Navigate(new AddBookPage());
        }

        private void Btn_Edit(object s, RoutedEventArgs e)
        {
            var button = s as Button;
            var book = button != null ? button.Tag as Books : null;
            if (book == null) return;

            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null) mainWindow.MainFrame.Navigate(new AddBookPage(book));
        }

        private void Btn_Dispute(object s, RoutedEventArgs e)
        {
            var button = s as Button;
            var book = button != null ? button.Tag as Books : null;
            if (book == null) return;

            bool exists = Core.Context.UnfreezeRequests.Any(r => r.UserId == App.CurrentUser.UserId && r.BookId == book.BookId && r.Status == "Pending");
            if (exists)
            {
                MessageBox.Show("Заявка уже отправлена");
                return;
            }

            Core.Context.UnfreezeRequests.Add(new UnfreezeRequests { UserId = App.CurrentUser.UserId, BookId = book.BookId, Reason = "Прошу разморозить книгу", Status = "Pending", CreatedAt = DateTime.Now });
            Core.Context.SaveChanges();
            MessageBox.Show("Заявка отправлена");
        }
    }
}