using System;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using static System.Reflection.Metadata.BlobBuilder;


static class bebebe
{
    public enum CategoryType
    {
        Detective = 1,
        Horror,
        Fantasy
    }
}
class Book
{
    public int BookID;
    public string name;
    public string author;
    public bebebe.CategoryType genre;
    public int year;
    public double price;
    public void PrintInfo()
    {
        Console.WriteLine($"ID товара: {BookID} \nНазвание: {name} \nЦена: {price} \nЖанр: {genre} \nАвтор: {author} \nГод выпуска: {year}");
    }
    public Book(int BookID, string name, string author, bebebe.CategoryType gen, int year, double price)
    {
        this.BookID = BookID;
        this.name = name;
        this.author = author;
        this.genre = gen;
        this.year = year;
        this.price = price;
    }
}
class Program
{
    static List<Book> books = new List<Book>();
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
            Console.WriteLine("6. Группировка по авторам");
            Console.WriteLine("0. Выход");
            Console.Write("Выберите пункт меню: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Add();
                    break;
                case "2":
                    Delete();
                    break;
                case "3":
                    Sort();
                    break;
                case "4":
                    MinMax();
                    break;
                case "5":
                    Search();
                    break;
                case "6":
                    GroupByAuthor();
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
    static void Add()
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
            books.Add(new Book(ID, name, author, gen, year, price));
            Console.WriteLine();
        }
    }

    static void Delete()
    {
        Console.WriteLine("Введите ID книги для удаления:");
        int idToDelete = Convert.ToInt32(Console.ReadLine());
        Book productToRemove = books.Find(p => p.BookID == idToDelete);

        if (productToRemove != null)
        {
            books.Remove(productToRemove);
            Console.WriteLine($"Книга с ID {idToDelete} удалена!");
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("Книга с таким ID не найдена!");
        }
    }
    static void Sort()
    {
        Console.WriteLine("\nСортировать по: ");
        Console.WriteLine("1. Названию");
        Console.WriteLine("2. Году издания");
        Console.Write("Выберите вариант: ");
        string sortChoice = Console.ReadLine();
        List<Book> sortedBooks = books;
        switch (sortChoice)
        {
            case "1":
                sortedBooks = books.OrderBy(b => b.name).ToList();
                Console.WriteLine("\nКниги, отсортированные по названию:");
                break;
            case "2":
                sortedBooks = books.OrderBy(b => b.year).ToList();
                Console.WriteLine("\nКниги, отсортированные по году издания:");
                break;
            default:
                Console.WriteLine("Неверный выбор сортировки!");
                return;
        }
        foreach (Book book in sortedBooks)
        {
            book.PrintInfo();
        }
    }
    static void MinMax()
    {
        var maxPriceBook = books.OrderByDescending(b => b.price).First();
        var minPriceBook = books.OrderBy(b => b.price).First();
        Console.WriteLine("\nСамая дорогая книга:");
        maxPriceBook.PrintInfo();
        Console.WriteLine("Самая дешевая книга:");
        minPriceBook.PrintInfo();
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
    static void GroupByAuthor()
    { }
    }


