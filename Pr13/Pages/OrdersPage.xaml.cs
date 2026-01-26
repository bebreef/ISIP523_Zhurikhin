using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pr13.Pages
{
    /// <summary>
    /// Логика взаимодействия для OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Page
    {
        public OrdersPage()
        {
            InitializeComponent();
            Loaded += OrdersPage_Loaded;
        }
        private void OrdersPage_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateCartText();
        }
        private void UpdateNextButton(bool cartIsEmpty)
        {
            if (Window.GetWindow(this) is MainWindow mw)
            {
                mw.SetNextButtonEnabled(!cartIsEmpty);
            }
        }
        private void UpdateCartText()
        {
            bool cartIsEmpty = Cart.Items.Count == 0;

            if (cartIsEmpty)
            {
                CartText.Text = "Корзина пуста. Добавьте товары, чтобы оформить заказ.";
            }
            else
            {
                var sb = new StringBuilder();

                foreach (var item in Cart.Items)
                {
                    sb.AppendLine($"{item.Title}");
                    sb.AppendLine($"   Цена: {item.Price:N0} ₽ × {item.Quantity} шт.");
                    sb.AppendLine($"   Сумма: {item.Price * item.Quantity:N0} ₽");
                    sb.AppendLine();
                }

                sb.AppendLine($"Итого: {Cart.TotalAmount:N0} ₽");
                CartText.Text = sb.ToString();
            }

            UpdateNextButton(cartIsEmpty);
        }
    }
}

