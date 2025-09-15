// See https://aka.ms/new-console-template for more information
using System.Diagnostics.CodeAnalysis;

int kolvo=0;
bool validInput = false;
while (!validInput)
{
    Console.WriteLine("Сколько у вас было операций за день (2-40)?");
    string input = Console.ReadLine();
    if (int.TryParse(input, out kolvo) && kolvo >= 2 && kolvo <= 40)
    {
        validInput = true;
    }
    else
    {
        Console.WriteLine("Некорректное количество операций, пожалуйста введите заново: ");
    }
}

Console.WriteLine("Введите операции по шаблону:");
Console.WriteLine("Название услуги или товара; Количество денег");
string[] names = new string[kolvo];
int[] sum = new int[kolvo];

for (int i = 0; i < kolvo; i++)
{
    bool validOperation = false;
    while (!validOperation)
    {
        string input = Console.ReadLine();
        string[] parts = input.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 2 && int.TryParse(parts[1].Trim(), out int amount))
        {
            sum[i] = amount;
            names[i] = parts[0].Trim();
            validOperation = true;
        }
        else
        {
            Console.WriteLine("Неверный формат! Введите по шаблону: Название; Сумма");
        }
    }
}

bool cont = true;
while (cont)
{
    Console.WriteLine("\n=== МЕНЮ ===");
    Console.WriteLine("1. Вывод данных");
    Console.WriteLine("2. Статистика (среднее, максимальное, минимальное, сумма)");
    Console.WriteLine("3. Сортировка по цене (пузырьковая сортировка)");
    Console.WriteLine("4. Конвертация валюты");
    Console.WriteLine("5. Поиск по названию");
    Console.WriteLine("0. Выход");
    Console.Write("Выберите пункт меню: ");

    string choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            for (int i = 0; i < kolvo; i++)
            {
                Console.WriteLine($"Операция {i + 1}:");
                Console.Write($"Название: {names[i]}. Стоимость: {sum[i]}.");
                Console.WriteLine("");
                Console.WriteLine("------------------------------------");
            }
            break;
        case "2":
            double total = 0;
            int max = sum[0];
            int min = sum[0];
            foreach (int a in sum)
            {
                total += a;
                if (a > max) max = a;
                if (a < min) min = a;
            }
            double average = total / sum.Length;
            Console.WriteLine("Статистика:");
            Console.WriteLine($"Общая сумма: {total} руб.");
            Console.WriteLine($"Средняя трата: {average:F2} руб.");
            Console.WriteLine($"Максимальная трата: {max} руб.");
            Console.WriteLine($"Минимальная трата: {min} руб.");
            break;
        case "3":
            for (int i = 0; i < sum.Length - 1; i++)
            {
                for (int j = 0; j < sum.Length - 1 - i; j++)
                {
                    if (sum[j] > sum[j + 1])
                    {
                        int temp = sum[j];
                        sum[j] = sum[j + 1];
                        sum[j + 1] = temp;
                        string temp1 = names[j];
                        names[j] = names[j + 1];
                        names[j + 1] = temp1;
                    }
                }
            }
            break;
        case "4":
            Console.WriteLine("Конвертер валют:");
            Console.WriteLine("1. Доллары (USD)");
            Console.WriteLine("2. Евро (EUR)");
            Console.WriteLine("Выберите валюту: ");
            int currencyChoice = Convert.ToInt32(Console.ReadLine());
            string currencyName="";
            if (currencyChoice == 1)
            {
                currencyName = "USD";
            }
            else if (currencyChoice == 2)
            {
                currencyName = "EUR";
            }
            Console.WriteLine($"Введите курс рубля к {currencyName} (сколько рублей за 1 {currencyName}): ");
            double rate=Convert.ToDouble(Console.ReadLine());
            Console.WriteLine($"Конвертация в {currencyName} по курсу: 1 {currencyName} = {rate} руб.");
            Console.WriteLine("Суммы в выбранной валюте:");
            Console.WriteLine("------------------------");
            double totalconv = 0;
            for (int i = 0; i < sum.Length; i++)
            {
                double conv = sum[i] / rate;
                totalconv += conv;
                Console.WriteLine($"{i + 1}. {conv} {currencyName}");
            }
            Console.WriteLine("------------------------");
            Console.WriteLine($"Итого: {totalconv} {currencyName}");
            break;
        case "5":
            Console.Write("Введите часть названия для поиска: ");
            string search = Console.ReadLine().ToLower();
            bool found = false;
            Console.WriteLine("Результаты поиска:");
            for (int i = 0; i < names.Length; i++)
            {
                if (names[i].ToLower().Contains(search))
                {
                    Console.WriteLine($"{names[i]} - {sum[i]} руб.");
                    found = true;
                }
            }
            if (!found)
            {
                Console.WriteLine("Траты с таким названием нет");
            }
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




