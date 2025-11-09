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
        static void Main(string[] args)
        {
        }
    }
}
