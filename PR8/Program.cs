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
        static void ShowMenu()
        {
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("GMWOG — МАРКЕТПЛЕЙС");
            Console.WriteLine("1. Регистрация");
            Console.WriteLine("2. Вход");
            Console.WriteLine("3. Каталог товаров");
            Console.WriteLine("4. Корзина");
            Console.WriteLine("5. Оформить заказ");
            Console.WriteLine("6. История заказов");
            Console.WriteLine("0. Выход");
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine($"Пользователь: {currentUser.Login}");
            Console.Write("Выбор: ");
        }
        static void Register()
        {
            int newId = 1;
            if (CorePR8.Context.Users.Any())
            {
                newId = CorePR8.Context.Users.Max(u => u.ID) + 1;
            }

            Console.Write("Введите логин: ");
            string login = Console.ReadLine().Trim();

            if (CorePR8.Context.Users.Any(u => u.Login == login))
            {
                Console.WriteLine("Такой логин уже существует!");
                return;
            }

            Console.Write("Введите пароль: ");
            string pass1 = Console.ReadLine();
            Console.Write("Повторите пароль: ");
            string pass2 = Console.ReadLine();

            if (pass1 != pass2)
            {
                Console.WriteLine("Пароли не совпадают!");
                return;
            }

            var user = new Users { ID = newId, Login = login, Password = pass1 };
            CorePR8.Context.Users.Add(user);
            CorePR8.Context.SaveChanges();
            Console.WriteLine("Регистрация успешна!");
        }
        static Users Login()
        {
            Console.Write("Введите логин: ");
            string login = Console.ReadLine().Trim();
            Console.Write("Введите пароль: ");
            string password = Console.ReadLine();

            var user = CorePR8.Context.Users
                .FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user == null)
            {
                Console.WriteLine("Неверный логин или пароль!");
                return null;
            }

            Console.WriteLine($"Добро пожаловать, {user.Login}!");
            return user;
        }
        static void ViewingProducts()
        {
            Console.WriteLine("\nКаталог товаров:");
            Console.WriteLine("--------------------------------------------");
            foreach (var p in CorePR8.Context.Products)
            {
                Console.WriteLine($"{p.ID}. {p.Name}");
                Console.WriteLine($"   {p.Description} — {p.Price:F0}₽");
            }

            if (currentUser == null)
            {
                Console.WriteLine("\nАвторизуйтесь для добавления в корзину.");
                return;
            }

            Console.Write("\nДобавить товар в корзину? (да/нет): ");
            if (Console.ReadLine().Trim().ToLower() == "да")
                AddToCart();
        }
        static void AddToCart()
        {
            if (currentUser == null) { Console.WriteLine("Сначала авторизуйтесь!"); return; }

            var cart = CorePR8.Context.Orders
                .FirstOrDefault(o => o.UserID == currentUser.ID && o.Status == 0);

            if (cart == null)
            {
                int newId = CorePR8.Context.Orders.Any() ? CorePR8.Context.Orders.Max(o => o.ID) + 1 : 1;
                cart = new Orders { ID = newId, UserID = currentUser.ID, PickupPointID = 0, Status = 0 };
                CorePR8.Context.Orders.Add(cart);
                CorePR8.Context.SaveChanges();
            }

            Console.Write("ID товара: ");
            int productId = int.Parse(Console.ReadLine().Trim());

            var product = CorePR8.Context.Products.Find(productId);
            if (product == null)
            {
                Console.WriteLine("Товар не найден!");
                return;
            }

            Console.Write("Количество: ");
            int qty = int.Parse(Console.ReadLine().Trim());

            var item = CorePR8.Context.OrderItems
                .FirstOrDefault(oi => oi.OrderID == cart.ID && oi.ProductID == productId);

            if (item != null)
                item.Quantity += qty;
            else
                CorePR8.Context.OrderItems.Add(new OrderItems
                {
                    OrderID = cart.ID,
                    ProductID = productId,
                    Quantity = qty,
                    PriceAtTime = product.Price
                });

            CorePR8.Context.SaveChanges();
            Console.WriteLine($"Добавлено {qty} × {product.Name} в корзину!");
        }

        static void Main(string[] args)
        {
        }
    }
}
