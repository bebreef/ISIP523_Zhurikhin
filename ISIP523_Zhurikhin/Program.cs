using System;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;


static class bebebe
{
    public enum CategoryType
    {
        Detective = 1,
        Horror,
        Fantasy
    }
}
class Product
{
    public int ProductID;
    public string name;
    public string author;
    public bebebe.CategoryType genre;
    public int year;
    public double price;
    public void PrintInfo()
    {
        Console.WriteLine($"ID товара: {ProductID} \nНазвание: {name} \nЦена: {price} \nЖанр: {genre} \nАвтор: {author} \nГод выпуска: {year}");
    }
    public Product(int ProductID, string name, string author, bebebe.CategoryType gen, int year, double price)
    {
        this.ProductID = ProductID;
        this.name = name;
        this.author = author;
        this.genre = gen;
        this.year = year;
        this.price = price;
    }
}
class Program
{
    static List<Product> prod = new List<Product>();
    static int ID = 0;
    static void Main(string[] args)
    {
        bool cont = true;
        while (cont)
        {
            Console.WriteLine("\n=== МЕНЮ ===");
            Console.WriteLine("1. Добавить книгу");
            Console.WriteLine("2. Удалить книгу");
            Console.WriteLine("3. Отсортировать книги"); // название и год
            Console.WriteLine("4. Самая дешевая и дорогая книга в коллекции");
            Console.WriteLine("5. Поиск"); //жанр, автор, название, айди
            Console.WriteLine("0. Выход");
            Console.Write("Выберите пункт меню: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddProduct();
                    break;
                case "2":
                    DeleteProduct();
                    break;
                case "3":
                    SupplyProduct();
                    break;
                case "4":
                    SellProduct();
                    break;
                case "5":
                    SearchProduct();
                    break;
                case "0":
                    Console.WriteLine("Завершение программы");
                    return;
                default:
                    Console.WriteLine("Неверный выбор");
                    break;
            }
            Console.WriteLine("Хотите вернуться в меню? (1-да, 0-нет)");
            string end = Console.ReadLine();
            cont = (end == "1");
        }
    }
    static void AddProduct()
    {
        int kolvo = 0;
        Console.WriteLine("Сколько книг вы хотите добавить?");
        kolvo = Convert.ToInt32(Console.ReadLine());
        for (int i = 0; i < kolvo; i++)
        {
            Console.WriteLine("Введите информацию о книге(название, автор, жанр, год выпуска, цена) \nПримечание к жанру: 1-детектив, 2-страшилка пугалка, 3-фантастика.");
            ID++;
            string name = Console.ReadLine();
            string author = Console.ReadLine();
            int y = Convert.ToInt32(Console.ReadLine());
            bebebe.CategoryType gen = (bebebe.CategoryType)y;
            int year = Convert.ToInt32(Console.ReadLine());
            double price = Convert.ToDouble(Console.ReadLine());
            prod.Add(new Product(ID, name, author, gen, year, price));
            Console.WriteLine();
        }
    }

    static void DeleteProduct()
    {
        Console.WriteLine("Введите ID книги для удаления:");
        int idToDelete = Convert.ToInt32(Console.ReadLine());
        Product productToRemove = prod.Find(p => p.ProductID == idToDelete);

        if (productToRemove != null)
        {
            prod.Remove(productToRemove);
            Console.WriteLine($"Книга с ID {idToDelete} удалена!");
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("Книга с таким ID не найдена!");
        }
    }

    static void SupplyProduct()
    {
        Console.WriteLine("Введите ID товара, которому необходима поставка");
        int addid = Convert.ToInt32(Console.ReadLine());
        Product addpr = prod.Find(p => p.ProductID == addid);

        if (addpr != null)
        {
            Console.WriteLine("Сколько штук товара пришло в поставке:");
            int supplyQuantity = Convert.ToInt32(Console.ReadLine());
            addpr.quantity += supplyQuantity;

            if (addpr.quantity > 0)
            {
                addpr.instock = true;
            }
        }
        else
        {
            Console.WriteLine("Товар с таким ID не найден!");
        }
    }

    static void SellProduct()
    {
        Console.WriteLine("Введите ID товара, который был продан");
        int prodid = Convert.ToInt32(Console.ReadLine());
        Product prodpr = prod.Find(p => p.ProductID == prodid);

        if (prodpr != null)
        {
            Console.WriteLine("Сколько штук было продано:");
            int prodQuantity = Convert.ToInt32(Console.ReadLine());

            if (prodQuantity > prodpr.quantity)
            {
                Console.WriteLine("КАК МОЖНО БЫЛО ПРОДАТЬ ТО, ЧЕГО НЕ СУЩЕСТВУЕТ");
                return;
            }

            prodpr.quantity -= prodQuantity;
            prodpr.instock = (prodpr.quantity > 0);
        }
        else
        {
            Console.WriteLine("Товар с таким ID не найден!");
        }
    }

    static void SearchProduct()
    {
        Console.WriteLine("Введите название товара для поиска (или нажмите Enter для вывода всех товаров):");
        string searchName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(searchName))
        {
            if (prod.Count == 0)
            {
                Console.WriteLine("Список товаров пуст.");
            }
            else
            {
                Console.WriteLine("\nВсе товары:");
                foreach (Product product in prod)
                {
                    product.PrintInfo();
                    Console.WriteLine("---------------");
                }
            }
        }
        else
        {
            List<Product> foundProducts = new List<Product>();

            foreach (Product product in prod)
            {
                if (product.name.ToLower().Contains(searchName.ToLower()))
                {
                    foundProducts.Add(product);
                }
            }

            if (foundProducts.Count == 0)
            {
                Console.WriteLine($"Товары с названием '{searchName}' не найдены.");
            }
            else
            {
                Console.WriteLine($"\nНайдено товаров: {foundProducts.Count}");
                foreach (Product product in foundProducts)
                {
                    product.PrintInfo();
                    Console.WriteLine("---------------");
                }
            }
        }
    }
}


