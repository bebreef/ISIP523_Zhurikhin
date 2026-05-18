using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace UPShootAndBunny.Pages
{
    public partial class AddBookPage : Page
    {
        private Books _book;

        public AddBookPage(Books book = null)
        {
            InitializeComponent();
            _book = book;
            LoadBook();
        }

        private void LoadBook()
        {
            if (_book == null) return;

            TbTitle.Text = _book.Title;
            TbDesc.Text = _book.Description;
            TbCover.Text = _book.CoverPath;
            TbContent.Text = _book.Content;
            BtnSave.Content = "Обновить";
        }

        private void Btn_SelectCover(object s, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "Изображения|*.jpg;*.jpeg;*.png", Title = "Выберите обложку книги" };

            if (dialog.ShowDialog() != true) return;

            string sourcePath = dialog.FileName;
            string extension = Path.GetExtension(sourcePath).ToLower();

            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
            {
                MessageBox.Show("Поддерживаются только форматы: .jpg, .jpeg, .png");
                return;
            }

            string coversDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "covers");
            Directory.CreateDirectory(coversDir);

            string uniqueName = "book_" + Guid.NewGuid().ToString("N").Substring(0, 8) + extension;
            string destPath = Path.Combine(coversDir, uniqueName);

            try
            {
                File.Copy(sourcePath, destPath, true);
                TbCover.Text = new Uri(destPath).AbsoluteUri;
                MessageBox.Show("Обложка загружена");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки обложки: " + ex.Message);
            }
        }

        private void Btn_Save(object s, RoutedEventArgs e)
        {
            try
            {
                if (App.CurrentUser == null)
                {
                    MessageBox.Show("Сначала войдите в аккаунт");
                    return;
                }

                if (string.IsNullOrWhiteSpace(TbTitle.Text))
                {
                    MessageBox.Show("Введите название");
                    return;
                }

                if (string.IsNullOrWhiteSpace(TbCover.Text))
                {
                    MessageBox.Show("Выберите обложку");
                    return;
                }

                if (_book == null)
                {
                    _book = new Books { AuthorId = App.CurrentUser.UserId, CreatedAt = DateTime.Now, IsFrozen = false };
                    Core.Context.Books.Add(_book);
                }

                _book.Title = TbTitle.Text.Trim();
                _book.Description = TbDesc.Text.Trim();
                _book.CoverPath = TbCover.Text.Trim();
                _book.Content = string.IsNullOrWhiteSpace(TbContent.Text) ? "Текст отсутствует" : TbContent.Text;

                Core.Context.SaveChanges();
                MessageBox.Show("Книга сохранена");
                ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new AuthorPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message);
            }
        }

        private void Btn_Back(object s, RoutedEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack) NavigationService.GoBack();
        }
    }
}
