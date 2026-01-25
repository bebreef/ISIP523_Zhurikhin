using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pr13.Pages;

namespace Pr13
{
    public static class Cart
    {
        public static ObservableCollection<CartItem> Items { get; } = new ObservableCollection<CartItem>();

        public static void AddProduct(ProductViewModel product, int quantity = 1)
        {
            var existing = Items.FirstOrDefault(i => i.ProductId == product.Id);
            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                Items.Add(new CartItem
                {
                    ProductId = product.Id,
                    Title = product.Title,
                    Price = product.Price,
                    Quantity = quantity
                });
            }
        }

        public static decimal TotalAmount => Items.Sum(i => i.Price * i.Quantity);

        public static void Clear()
        {
            Items.Clear();
        }
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}