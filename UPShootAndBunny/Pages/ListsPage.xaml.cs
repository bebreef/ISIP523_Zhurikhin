using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UPShootAndBunny.Pages
{
    public partial class ListsPage : Page
    {
        private string _currentStatus = "В планах";
        private List<ReadingLists> _userLists = new List<ReadingLists>();

        public ListsPage()
        {
            InitializeComponent();
            LoadGenres();
            SetActiveTab(BtnPlans);
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
                var query = Core.Context.ReadingLists
                    .Include("Books")
                    .Include("Books.Users")
                    .Include("Books.Genres")
                    .Include("Books.Reviews")
                    .Where(r => r.UserId == App.CurrentUser.UserId && r.Status == _currentStatus);

                var lists = query.ToList();

                if (!string.IsNullOrWhiteSpace(TbSearch.Text))
                {
                    string search = TbSearch.Text.ToLower();
                    lists = lists.Where(r => r.Books != null &&
                        (r.Books.Title != null && r.Books.Title.ToLower().Contains(search) ||
                         r.Books.Users != null && r.Books.Users.DisplayName != null && r.Books.Users.DisplayName.ToLower().Contains(search))).ToList();
                }

                if (CbGenre.SelectedValue != null)
                {
                    int genreId = (int)CbGenre.SelectedValue;
                    lists = lists.Where(r => r.Books != null && r.Books.Genres != null && r.Books.Genres.Any(g => g.GenreId == genreId)).ToList();
                }

                var sortItem = CbSort.SelectedItem as ComboBoxItem;
                string sortValue = sortItem?.Content.ToString();

                if (sortValue == "По рейтингу")
                {
                    _userLists = lists.OrderByDescending(r =>
                    {
                        if (r.Books == null || r.Books.Reviews == null || !r.Books.Reviews.Any()) return 0;
                        return r.Books.Reviews.Average(rv => rv.Rating);
                    }).ToList();
                }
                else
                {
                    _userLists = lists.OrderBy(r => r.Books != null && r.Books.Title != null ? r.Books.Title : "").ToList();
                }

                BooksList.ItemsSource = _userLists;
            }
            catch { }
        }

        private void SetActiveTab(Button activeBtn)
        {
            foreach (var btn in new[] { BtnPlans, BtnReading, BtnRead, BtnDropped })
            {
                btn.Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E5E5EA"));
                btn.Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#8E8E93"));
            }
            activeBtn.Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#007AFF"));
            activeBtn.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
        }

        private void Tab_Click(object s, RoutedEventArgs e)
        {
            var btn = s as Button;
            if (btn?.Tag != null)
            {
                _currentStatus = btn.Tag.ToString();
                SetActiveTab(btn);
                LoadBooks();
            }
        }

        private void TbSearch_TextChanged(object s, TextChangedEventArgs e) { LoadBooks(); }
        private void CbSort_SelectionChanged(object s, SelectionChangedEventArgs e) { LoadBooks(); }
        private void CbGenre_SelectionChanged(object s, SelectionChangedEventArgs e) { LoadBooks(); }

        private void Btn_MoveBook(object s, RoutedEventArgs e)
        {
            try
            {
                var btn = s as Button;
                var readingList = btn?.Tag as ReadingLists;
                if (readingList == null) return;

                var menu = new ContextMenu();
                string[] statuses = { "В планах", "Читаю", "Прочитано", "Заброшено" };
                foreach (var st in statuses)
                {
                    if (st != readingList.Status)
                    {
                        var item = new MenuItem { Header = st };
                        item.Click += (sender, args) =>
                        {
                            readingList.Status = st;
                            readingList.AddedAt = DateTime.Now;
                            Core.Context.SaveChanges();
                            LoadBooks();
                        };
                        menu.Items.Add(item);
                    }
                }
                menu.PlacementTarget = btn;
                menu.IsOpen = true;
            }
            catch { }
        }
    }
}