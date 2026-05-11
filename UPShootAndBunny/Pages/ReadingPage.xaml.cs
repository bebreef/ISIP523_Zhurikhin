using System;
using System.Windows;
using System.Windows.Controls;

namespace UPShootAndBunny.Pages
{
    public partial class ReadingPage : Page
    {
        private int _fontSize = 18;
        public ReadingPage(Books book)
        {
            InitializeComponent();
            TbContent.Text = book?.Content?.Replace("\r\n", Environment.NewLine) ?? "Текст отсутствует";
        }
        private void Btn_DecreaseFont(object s, RoutedEventArgs e) { if (_fontSize > 12) { _fontSize -= 2; TbContent.FontSize = _fontSize; TbContent.LineHeight = _fontSize + 10; } }
        private void Btn_IncreaseFont(object s, RoutedEventArgs e) { if (_fontSize < 36) { _fontSize += 2; TbContent.FontSize = _fontSize; TbContent.LineHeight = _fontSize + 10; } }
        private void Btn_Back(object s, RoutedEventArgs e) { if (NavigationService?.CanGoBack == true) NavigationService.GoBack(); }
    }
}