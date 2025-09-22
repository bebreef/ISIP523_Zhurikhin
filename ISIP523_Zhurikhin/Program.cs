using System;
using static System.Net.Mime.MediaTypeNames;


static class bebebe
{
     public enum CategoryType
    {
        Food = 1,
        Clothes,
        Tech
    }
}
class Product
{
    public int ProductID;
    public string name;
    public double price;
    public int quantity;
    public bool instock;
    public bebebe.CategoryType category; 
    public void PrintInfo()
    {
        Console.WriteLine($"ID товара: {ProductID} \nНазвание: {name} \nЦена: {price} \nКоличество: {quantity} \nНаличие: {instock} \nКатегория: {category}");
    }
    public Product(int ProductID, string name, double price, int quan, bool instock, bebebe.CategoryType cat)
    {
        this.ProductID = ProductID;
        this.name = name;
        this.price = price;
        this.quantity = quan;
        this.instock = instock;
        this.category = cat;
    }
}
class Program
{
    static void Main(string[] args)
    {
        List<Product> prod = new List<Product>();
        bool cont = true;
        while (cont)
        {
            Console.WriteLine("\n=== МЕНЮ ===");
            Console.WriteLine("1. Добавить товар");
            Console.WriteLine("2. Удалить товар");
            Console.WriteLine("3. Заказать поставку товара");
            Console.WriteLine("4. Продать товар");
            Console.WriteLine("5. Поиск по названию");
            Console.WriteLine("0. Выход");
            Console.Write("Выберите пункт меню: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    int kolvo = 0;
                    Console.WriteLine("Сколько товаров вы хотите добавить?");
                    kolvo = Convert.ToInt32(Console.ReadLine());
                    for (int i = 0;i<kolvo; i++)
                    {
                        Console.WriteLine("Введите информацию о товаре(название, цена, количество, категория) \n Примечание к категориям: 1-еда, 2-одежда, 3-техника.");
                        int ID = i+1;
                        string name = Console.ReadLine();
                        double price = Convert.ToDouble(Console.ReadLine());
                        int quan = Convert.ToInt32(Console.ReadLine());
                        bool instock=false;
                        if (quan > 0)
                        {
                            instock = true;
                        }
                        int y = Convert.ToInt32(Console.ReadLine());
                        bebebe.CategoryType cat;
                        switch (y)
                        {
                            case 1:
                                cat = bebebe.CategoryType.Food;
                                prod.Add(new Product(ID, name, price, quan, instock, cat));
                                break;
                            case 2:
                                cat = bebebe.CategoryType.Clothes;
                                prod.Add(new Product(ID, name, price, quan, instock, cat));
                                break;
                            case 3:
                                cat = bebebe.CategoryType.Tech;
                                prod.Add(new Product(ID, name, price, quan, instock, cat));
                                break;
                        }
                        Console.WriteLine();
                    }
                    break;
                case "2":
                    break;
                case "3":
                    break;
                case "4":
                    break;
                case "5":
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
}

