using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR8
{
    public static class CorePR8
    {
        public static PR8Entities Context = new PR8Entities();
    }
    internal class Program
    {
        static Users currentUser;

        static void InitializeTestData()
        {
            if (!CorePR8.Context.Products.Any())
            {
                var products = new List<Products>
                {   new Products { ID = 1, Name = "Футболка GMWOG", Description = "Чёрная, 100% хлопок", Price = 1299.00m },
                    new Products { ID = 2, Name = "Кепка GMWOG", Description = "С вышивкой", Price = 899.00m },
                    new Products { ID = 3, Name = "Кружка GMWOG", Description = "Керамика, 330мл", Price = 499.00m }
                };
                CorePR8.Context.Products.AddRange(products);
                var pickuppoints = new List<PickupPoints>
                {   new PickupPoints { ID = 1, City = "Москва", Address = "ул. Ленина, 10" },
                    new PickupPoints { ID = 2, City = "Санкт-Петербург", Address = "пр. Невский, 25" },
                    new PickupPoints { ID = 3, City = "Казань", Address = "ул. Баумана, 5" }
                };
                CorePR8.Context.PickupPoints.AddRange(pickuppoints);
                CorePR8.Context.SaveChanges();
                Console.WriteLine("Тестовые данные загружены!");
            }
        }
        static void Main(string[] args)
        {
        }
    }
}
