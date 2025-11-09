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
            Console.WriteLine(currentUser != null ? $"Пользователь: {currentUser.Login}" : "Гость");
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
            string answer = Console.ReadLine().Trim().ToLower();
            if (answer == "да")
            {
                AddToCart();  
            }
        }
        static void AddToCart()
        {
            if (currentUser == null) { Console.WriteLine("Сначала авторизуйтесь!"); return; }

            var cart = CorePR8.Context.Orders
                .FirstOrDefault(o => o.UserID == currentUser.ID && o.Status == 0);

            if (cart == null)
            {
                int newId = CorePR8.Context.Orders.Any() ? CorePR8.Context.Orders.Max(o => o.ID) + 1 : 1;
                cart = new Orders { ID = newId, UserID = currentUser.ID, Status = 0 };
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
        static void ViewingCart()
        {
            if (currentUser == null) { Console.WriteLine("Сначала авторизуйтесь!"); return; }

            var cart = CorePR8.Context.Orders
                .FirstOrDefault(o => o.UserID == currentUser.ID && o.Status == 0);

            if (cart == null || !CorePR8.Context.OrderItems.Any(oi => oi.OrderID == cart.ID))
            {
                Console.WriteLine("Корзина пуста!");
                return;
            }

            var items = CorePR8.Context.OrderItems
                .Where(oi => oi.OrderID == cart.ID)
                .Join(CorePR8.Context.Products,
                    oi => oi.ProductID,
                    p => p.ID,
                    (oi, p) => new { p.Name, oi.Quantity, oi.PriceAtTime })
                .ToList();

            Console.WriteLine("\nКорзина:");
            Console.WriteLine();
            decimal total = 0;
            foreach (var item in items)
            {
                decimal sum = item.PriceAtTime * item.Quantity;
                Console.WriteLine($"{item.Name} × {item.Quantity} = {sum:F0}₽");
                total += sum;
            }
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine($"Итого: {total}₽");
        }
        static void PlaceAnOrder()
        {
            if (currentUser == null) { Console.WriteLine("Сначала авторизуйтесь!"); return; }

            var cart = CorePR8.Context.Orders
                .FirstOrDefault(o => o.UserID == currentUser.ID && o.Status == 0);

            if (cart == null || !CorePR8.Context.OrderItems.Any(oi => oi.OrderID == cart.ID))
            {
                Console.WriteLine("Корзина пуста!");
                return;
            }

            var cartItems = CorePR8.Context.OrderItems.Where(oi => oi.OrderID == cart.ID).ToList();

            Console.WriteLine("\n1. Один товар");
            Console.WriteLine("2. Всю корзину");
            Console.Write("Выбор: ");
            string choice = Console.ReadLine().Trim();

            List<OrderItems> toBuy;
            if (choice == "1")
            {
                Console.WriteLine("\nТовары в корзине:");
                foreach (var item in cartItems)
                {
                    var p = CorePR8.Context.Products.Find(item.ProductID);
                    Console.WriteLine($"{p.ID}. {p.Name} × {item.Quantity}");
                }
                Console.Write("ID товара: ");
                int id = int.Parse(Console.ReadLine().Trim());
                var selected = cartItems.First(i => CorePR8.Context.Products.Find(i.ProductID).ID == id);
                toBuy = new List<OrderItems> { selected };
            }
            else
            {
                toBuy = cartItems;
            }

            Console.WriteLine("\nПункты выдачи:");
            foreach (var pvz in CorePR8.Context.PickupPoints)
                Console.WriteLine($"{pvz.ID}. {pvz.City}: {pvz.Address}");

            Console.Write("ID ПВЗ: ");
            int pvzId = int.Parse(Console.ReadLine().Trim());

            cart.PickupPointID = pvzId;
            cart.Status = 1;

            if (choice == "1")
            {
                CorePR8.Context.OrderItems.Remove(toBuy[0]);
                if (!CorePR8.Context.OrderItems.Any(oi => oi.OrderID == cart.ID))
                    CorePR8.Context.Orders.Remove(cart);
            }
            else
            {
                int newCartId = CorePR8.Context.Orders.Any() ? CorePR8.Context.Orders.Max(o => o.ID) + 1 : 1;
                CorePR8.Context.Orders.Add(new Orders
                {
                    ID = newCartId,
                    UserID = currentUser.ID,
                    Status = 0
                });
            }

            CorePR8.Context.SaveChanges();
            Console.WriteLine("Заказ оформлен успешно!");
        }
        static void ShowOrders()
        {
            if (currentUser == null) { Console.WriteLine("Сначала авторизуйтесь!"); return; }

            var orders = CorePR8.Context.Orders
                .Where(o => o.UserID == currentUser.ID && o.Status == 1)
                .OrderByDescending(o => o.ID)
                .ToList();

            if (!orders.Any())
            {
                Console.WriteLine("У вас нет заказов.");
                return;
            }

            Console.WriteLine("\nИстория заказов:");
            Console.WriteLine("--------------------------------------------");

            foreach (var order in orders)
            {
                var pvz = CorePR8.Context.PickupPoints.Find(order.PickupPointID);
                Console.WriteLine($"\nЗаказ #{order.ID}");
                Console.WriteLine($"ПВЗ: {pvz.City}, {pvz.Address}");

                var items = CorePR8.Context.OrderItems
                    .Where(oi => oi.OrderID == order.ID)
                    .Join(CorePR8.Context.Products,
                        oi => oi.ProductID,
                        p => p.ID,
                        (oi, p) => new { p.Name, oi.Quantity, oi.PriceAtTime })
                    .ToList();

                decimal total = 0;
                foreach (var item in items)
                {
                    decimal sum = item.PriceAtTime * item.Quantity;
                    Console.WriteLine($"  {item.Name} × {item.Quantity} = {sum}₽");
                    total += sum;
                }
                Console.WriteLine($"  Итого: {total}₽");
            }
        }
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Добро пожаловать в GMWOG маркетплейс!");
                InitializeTestData();

                bool running = true;
                while (running)
                {
                    ShowMenu();
                    string choice = Console.ReadLine().Trim();

                    switch (choice)
                    {
                        case "1": Register(); break;
                        case "2": currentUser = Login(); break;
                        case "3": ViewingProducts(); break;
                        case "4": ViewingCart(); break;
                        case "5": PlaceAnOrder(); break;
                        case "6": ShowOrders(); break;
                        case "0": running = false; Console.WriteLine("Мы всё ещё ждём ваших денег."); break;
                        default: Console.WriteLine("Неверный выбор!"); break;
                    }
                }
            }
        }
    }
