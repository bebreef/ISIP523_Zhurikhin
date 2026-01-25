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
    public partial class ProductsPage : Page
    {
        public ObservableCollection<ProductViewModel> Products { get; set; }
            = new ObservableCollection<ProductViewModel>();

        public ProductsPage()
        {
            InitializeComponent();
            DataContext = this;
            LoadProducts();
        }
        private void LoadProducts()
        {
            var dbProducts = Core.Context.Products.ToList();   

            foreach (var p in dbProducts)
            {
                Products.Add(new ProductViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Price = p.Price,
                    ImagePath = $"/Images/{p.Id}.png"
                });
            }
        }
        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ProductViewModel product)
            {
                Cart.AddProduct(product, 1);  

                MessageBox.Show($"{product.Title} добавлен в корзину!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
    }
}

