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
            if (_book != null)
            {
                TbTitle.Text = _book.Title;
                TbDesc.Text = _book.Description;
                TbCover.Text = _book.CoverPath;
                TbContent.Text = _book.Content;
                BtnSave.Content = "Обновить";
            }
        }

        private void Btn_SelectCover(object s, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png",
                Title = "Выберите обложку книги"
            };

            if (dlg.ShowDialog() == true)
            {
                string sourcePath = dlg.FileName;
                string fileName = Path.GetFileName(sourcePath);
                string ext = Path.GetExtension(fileName).ToLower();

                if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                {
                    MessageBox.Show("⚠ Поддерживаются только форматы: .jpg, .jpeg, .png");
                    return;
                }

                string coversDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Covers");
                if (!Directory.Exists(coversDir)) Directory.CreateDirectory(coversDir);

                string uniqueName = $"book_{Guid.NewGuid().ToString().Substring(0, 8)}{ext}";
                string destPath = Path.Combine(coversDir, uniqueName);

                try
                {
                    File.Copy(sourcePath, destPath, true);
                    TbCover.Text = $"/Covers/{uniqueName}";
                    MessageBox.Show("✓ Обложка успешно загружена!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"✗ Ошибка: {ex.Message}");
                }
            }
        }

        private void Btn_Save(object s, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TbTitle.Text)) { MessageBox.Show("Введите название"); return; }
                if (string.IsNullOrWhiteSpace(TbCover.Text)) { MessageBox.Show("Выберите обложку"); return; }

                if (_book == null)
                {
                    _book = new Books { AuthorId = App.CurrentUser.UserId, CreatedAt = DateTime.Now, IsFrozen = false };
                    Core.Context.Books.Add(_book);
                }

                _book.Title = TbTitle.Text;
                _book.Description = TbDesc.Text;
                _book.CoverPath = TbCover.Text;
                _book.Content = TbContent.Text;

                Core.Context.SaveChanges();
                MessageBox.Show("✓ Успешно сохранено!");
                ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new AuthorPage());
            }
            catch (Exception ex) { MessageBox.Show($"✗ Ошибка: {ex.Message}"); }
        }

        private void Btn_Back(object s, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true) NavigationService.GoBack();
        }
    }
}