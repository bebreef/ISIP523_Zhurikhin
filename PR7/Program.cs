using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PR7;

namespace PR7
{
    public static class CorePR7
    {
        public static PR7Entities Context { get; } = new PR7Entities();
    }

    internal class Program
    {
        private static int clientCounter = 1; 

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            try
            {
                int carsServiced = 0;
                List<(int PartID, int Quantity, int Remaining)> pendingDeliveries = new List<(int, int, int)>();

                Console.WriteLine("Добро пожаловать в автосервис!");
                Console.Write("Введите имя игрока: ");
                string playerName = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(playerName)) playerName = "Игрок";

                var player = new Players
                {
                    Name = playerName,
                    Balance = 5000,
                    TotalRepairs = 0,
                    TotalFines = 0
                };
                CorePR7.Context.Players.Add(player);
                CorePR7.Context.SaveChanges();

                Console.WriteLine($"Игрок {player.Name} создан! Баланс: {player.Balance}");

                var parts = CorePR7.Context.Parts.ToList();
                foreach (var part in parts)
                {
                    if (!CorePR7.Context.Storage.Any(s => s.PlayerID == player.ID && s.PartID == part.ID))
                    {
                        CorePR7.Context.Storage.Add(new Storage
                        {
                            PlayerID = player.ID,
                            PartID = part.ID,
                            Quantity = 1
                        });
                    }
                }
                CorePR7.Context.SaveChanges();

                bool game = true;
                while (game)
                {
                    ShowMenu(player, carsServiced, pendingDeliveries.Count);
                    string choice = Console.ReadLine().Trim();

                    switch (choice)
                    {
                        case "1":
                            ServiceClient(player, ref carsServiced, pendingDeliveries);
                            break;
                        case "2":
                            PurchaseParts(player, pendingDeliveries);
                            break;
                        case "3":
                            CheckPendingDeliveries(pendingDeliveries);
                            break;
                        case "4":
                            ShowStorage(player);
                            break;
                        case "5":
                            game = false;
                            Console.WriteLine("Игра завершена!");
                            break;
                        default:
                            Console.WriteLine("Неверный выбор!");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Console.WriteLine("Нажмите любую клавишу...");
                Console.ReadKey();
            }
        }

        static void ShowMenu(Players player, int carsServiced, int pendingCount)
        {
            Console.WriteLine("\n" + new string('=', 40));
            Console.WriteLine("МЕНЮ");
            Console.WriteLine("1. Новый клиент");
            Console.WriteLine("2. Закупить детали");
            Console.WriteLine("3. Проверить доставки");
            Console.WriteLine("4. Показать склад");
            Console.WriteLine("5. Выход");
            Console.WriteLine($"Баланс: {player.Balance:F0} | Обслужено: {carsServiced} | Штрафы: {player.TotalFines} | Доставок: {pendingCount}");
            Console.Write("Выбор: ");
        }

        static void ServiceClient(Players player, ref int carsServiced,
                                  List<(int PartID, int Quantity, int Remaining)> pendingDeliveries)
        {
            var random = new Random();
            var carModels = new[] { "Lada", "Toyota", "DMC DeLorean", "Audi", "Ford" };
            var parts = CorePR7.Context.Parts.ToList();
            var brokenPart = parts[random.Next(parts.Count)];

            string clientName = $"Клиент {clientCounter++}";
            string carModel = carModels[random.Next(carModels.Length)];

            Console.WriteLine($"\nПриехал клиент: {clientName} ({carModel})");
            Console.WriteLine($"Сломалась деталь: {brokenPart.Name}");
            Console.Write("Принять заказ? (да/нет): ");
            if (Console.ReadLine()?.Trim().ToLower() != "да")
            {
                decimal fine = 200;
                player.Balance -= fine;
                player.TotalFines += (int)fine;

                CorePR7.Context.Orders.Add(new Orders
                {
                    PlayerID = player.ID,
                    ClientName = clientName,
                    CarModel = carModel,
                    BrokenPartID = brokenPart.ID,
                    InstalledPartID = null,
                    Status = "Отказано",
                    RepairPrice = 0,
                    FinePrice = fine
                });
                CorePR7.Context.SaveChanges();

                Console.WriteLine($"Штраф: {fine}. Баланс: {player.Balance:F0}");
                carsServiced++;
                UpdateDeliveries(player, pendingDeliveries);
                return;
            }

            var storage = CorePR7.Context.Storage
                .FirstOrDefault(s => s.PlayerID == player.ID && s.PartID == brokenPart.ID && s.Quantity > 0);

            if (storage != null)
            {
                storage.Quantity--;
                player.Balance += brokenPart.RepairPrice;
                player.TotalRepairs++;

                CorePR7.Context.Orders.Add(new Orders
                {
                    PlayerID = player.ID,
                    ClientName = clientName,
                    CarModel = carModel,
                    BrokenPartID = brokenPart.ID,
                    InstalledPartID = brokenPart.ID,
                    Status = "Выполнен",
                    RepairPrice = brokenPart.RepairPrice,
                    FinePrice = 0
                });
                CorePR7.Context.SaveChanges();

                Console.WriteLine($"Ремонт выполнен! +{brokenPart.RepairPrice}. Баланс: {player.Balance:F0}");
            }
            else
            {
                Console.WriteLine("Нет детали на складе. Клиент уехал.");
                CorePR7.Context.Orders.Add(new Orders
                {
                    PlayerID = player.ID,
                    ClientName = clientName,
                    CarModel = carModel,
                    BrokenPartID = brokenPart.ID,
                    InstalledPartID = null,
                    Status = "Нет детали",
                    RepairPrice = 0,
                    FinePrice = 0
                });
                CorePR7.Context.SaveChanges();
            }

            carsServiced++;
            UpdateDeliveries(player, pendingDeliveries);
        }

        static void UpdateDeliveries(Players player, List<(int PartID, int Quantity, int Remaining)> pendingDeliveries)
        {
            for (int i = 0; i < pendingDeliveries.Count; i++)
            {
                var d = pendingDeliveries[i];
                d.Remaining--;
                if (d.Remaining <= 0)
                {
                    var storage = CorePR7.Context.Storage
                        .FirstOrDefault(s => s.PlayerID == player.ID && s.PartID == d.PartID);
                    if (storage == null)
                    {
                        storage = new Storage
                        {
                            PlayerID = player.ID,
                            PartID = d.PartID,
                            Quantity = 0
                        };
                        CorePR7.Context.Storage.Add(storage);
                    }
                    storage.Quantity += d.Quantity;

                    var part = CorePR7.Context.Parts.First(p => p.ID == d.PartID);
                    Console.WriteLine($"Поставка прибыла: {d.Quantity} × {part.Name}");

                    pendingDeliveries.RemoveAt(i);
                    i--;
                }
                else
                {
                    pendingDeliveries[i] = d;
                }
            }
            CorePR7.Context.SaveChanges();
        }

        static void PurchaseParts(Players player, List<(int PartID, int Quantity, int Remaining)> pendingDeliveries)
        {
            Console.WriteLine("\nДоступные детали:");
            var parts = CorePR7.Context.Parts.ToList();
            for (int i = 0; i < parts.Count; i++)
                Console.WriteLine($"{i + 1}. {parts[i].Name} — {parts[i].PurchasePrice} руб.");

            Console.Write("Номер детали: ");
            if (!int.TryParse(Console.ReadLine(), out int idx) || idx < 1 || idx > parts.Count)
            {
                Console.WriteLine("Неверный ввод.");
                return;
            }

            var part = parts[idx - 1];
            Console.Write("Количество: ");
            if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0)
            {
                Console.WriteLine("Неверное количество.");
                return;
            }

            decimal cost = part.PurchasePrice * qty;
            if (player.Balance < cost)
            {
                Console.WriteLine("Недостаточно средств!");
                return;
            }

            player.Balance -= cost;
            pendingDeliveries.Add((part.ID, qty, 2));

            CorePR7.Context.PurchaseOrders.Add(new PurchaseOrders
            {
                PlayerID = player.ID,
                PartID = part.ID,
                Quantity = qty,
                Price = cost,
                DeliveryAfter = 2
            });
            CorePR7.Context.SaveChanges();

            Console.WriteLine($"Заказано {qty} × {part.Name} за {cost}. Прибудет через 2 клиента.");
        }

        static void CheckPendingDeliveries(List<(int PartID, int Quantity, int Remaining)> pendingDeliveries)
        {
            if (pendingDeliveries.Count == 0)
            {
                Console.WriteLine("Нет ожидаемых поставок.");
                return;
            }

            Console.WriteLine("Ожидаемые поставки:");
            foreach (var d in pendingDeliveries)
            {
                var part = CorePR7.Context.Parts.First(p => p.ID == d.PartID);
                Console.WriteLine($"{part.Name} × {d.Quantity} — через {d.Remaining} клиента(ов)");
            }
        }

        static void ShowStorage(Players player)
        {
            Console.WriteLine("\nВаш склад:");
            var items = CorePR7.Context.Storage
                .Where(s => s.PlayerID == player.ID && s.Quantity > 0)
                .Join(CorePR7.Context.Parts, s => s.PartID, p => p.ID, (s, p) => new { p.Name, s.Quantity })
                .ToList();

            if (items.Count == 0)
                Console.WriteLine("Склад пуст.");
            else
                foreach (var item in items)
                    Console.WriteLine($"{item.Name} — {item.Quantity} шт.");
        }
    }
}