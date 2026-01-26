using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для ConfirmPage.xaml
    /// </summary>
    public partial class ConfirmPage : Page
    {
        public ObservableCollection<CartItem> CartItems => Cart.Items;
        public ConfirmPage()
        {
            InitializeComponent();
            Loaded += ConfirmPage_Loaded;
            DataContext = this;
        }
        private void ConfirmPage_Loaded(object sender, RoutedEventArgs e)
        {
            bool a = false;
            if (Window.GetWindow(this) is MainWindow mw)
            {
                mw.SetNextButtonEnabled(a);
            }
        }
        private void ConfirmOrder_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbFullName.Text) ||
                string.IsNullOrWhiteSpace(tbEmail.Text) ||
                string.IsNullOrWhiteSpace(tbAddress.Text))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Cart.Items.Count == 0)
            {
                MessageBox.Show("Корзина пуста!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var order = new Orders
                {
                    CreatedDate = DateTime.Now,
                    FullName = tbFullName.Text.Trim(),
                    Email = tbEmail.Text.Trim(),
                    Address = tbAddress.Text.Trim(),
                    TotalAmount = Cart.TotalAmount,
                    Status = "Новый"
                };

                Core.Context.Orders.Add(order);
                Core.Context.SaveChanges(); 

                foreach (var item in Cart.Items)
                {
                    var orderItem = new OrderItems
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        PriceAtOrderTime = item.Price
                    };
                    Core.Context.OrderItems.Add(orderItem);
                }

                Core.Context.SaveChanges();

                MessageBox.Show("Заказ успешно оформлен!\nНомер заказа: " + order.Id,
                                "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                Cart.Clear();

                NavigationService?.Navigate(new ProductsPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении заказа:\n" + ex.Message,
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
