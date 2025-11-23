using System;
using ISIP523_Zhurikhin;

namespace ISIP523_Zhurikhin
{
    internal class Game
    {
        private Player player;
        private int turnCount = 0;

        public void start()
        {
            Console.WriteLine("Как вас будут звать?");
            string name = Console.ReadLine() ?? "Герой";

            Console.WriteLine("Выберите стартовое оружие. (1-Меч, 2-Клеймор, 3-Топор)");
            string choice = Console.ReadLine();
            Weapon startWeapon = choice switch
            {
                "1" => new Sword(),
                "2" => new Claymore(),
                "3" => new Axe(),
                _ => new Sword()
            };

            Console.WriteLine("Выберите броню. (1-Тяжелая Броня (+1 урон Меча), 2-Средняя Броня (+1 урон Клеймора), 3-Легкая Броня (+1 урон Топора))");
            string armorChoice = Console.ReadLine();
            Armor startArmor = armorChoice switch
            {
                "1" => new Armor { name = "Тяжелая Броня", defense = 1, bufftype = "Меч" },
                "2" => new Armor { name = "Средняя Броня", defense = 1, bufftype = "Клеймор" },
                "3" => new Armor { name = "Легкая Броня", defense = 1, bufftype = "Топор" },
                _ => new Armor { name = "Тяжелая Броня", defense = 1, bufftype = "Меч" }
            };

            player = new Player(name, 20, startWeapon, startArmor);
            RunGame();
        }

        private void RunGame()
        {
            while (player.isAlive)
            {
                turnCount++;
                Console.WriteLine($"\nХод {turnCount}");
                DisplayStats();

                if (turnCount % 5 == 0)
                    FightBoss();
                else if (Random.NextDouble() < 0.3)
                    OpenChest();
                else
                    FightEnemy();

                Console.ReadLine();
                player.RegenerateStamina();
            }
            Console.WriteLine("Игра окончена!");
        }

        private void DisplayStats()
        {
            Console.WriteLine($"{player.Name}: HP={player.HP}/{player.MaxHP}, Выносливость={player.Stamina}/{player.MaxStamina}, Оружие={player.equippedweapon.name} (Прочность={player.equippedweapon.durability})");
        }

        private void FightEnemy()
        {
            Enemy enemy = EnemyFactory.CreateRegular();
            Console.WriteLine($"{enemy.Name} появляется!");
            Fight(player, enemy);
        }

        private void FightBoss()
        {
            Enemy boss = EnemyFactory.CreateBoss();
            Console.WriteLine($"БОСС: {boss.Name} появляется!");
            Fight(player, boss);
        }

        private void Fight(Player player, Enemy enemy)
        {
            while (player.isAlive && enemy.isAlive)
            {
                Console.WriteLine($"{enemy.Name}: HP={enemy.HP}/{enemy.MaxHP}");

                if (!player.isFrozen)
                {
                    Console.WriteLine("Выберите: 1-Аттаковать, 2-Защищаться");
                    string choice = Console.ReadLine()?.ToUpper() ?? "2";
                    player.isDefending = false;

                    if (choice == "1" && player.Stamina >= player.equippedweapon.staminacost)
                    {
                        player.Stamina -= player.equippedweapon.staminacost;
                        player.equippedweapon.durability = Math.Max(1, player.equippedweapon.durability - 1);
                        int damage = player.equippedweapon.dmg;
                        if (player.equippedarmor.bufftype == player.equippedweapon.name)
                            damage += 1;

                        enemy.TakeDamage(damage); 
                        Console.WriteLine($"{player.Name} атакует с помощью {player.equippedweapon.name} и наносит {damage} урона!"); 
                        Console.WriteLine("-------------------------------------");
                    }
                    else if (choice == "1")
                    {
                        Console.WriteLine("Недостаточно выносливости! Пропуск хода!");
                    }
                    else
                    {
                        player.isDefending = true;
                        Console.WriteLine($"{player.Name} защищается!");
                    }
                }
                else
                {
                    Console.WriteLine($"{player.Name} заморожен и пропускает ход!");
                    player.isFrozen = false;
                }

                if (enemy.isAlive)
                    enemy.AttackPlayer(player);

                if (!enemy.isAlive)
                    OpenChest();
            }

            if (!player.isAlive)
                Console.WriteLine("Вы погибли...");
        }

        private void OpenChest()
        {
            Console.WriteLine("Вы нашли сундук!");
            object item;
            int chestReward = Random.Next(3);

            switch (chestReward)
            {
                case 0:
                    item = new Food();
                    break;
                case 1:
                    int boost = Random.Next(1, 4);
                    item = new StaminaPotion
                    {
                        name = $"Зелье выносливости +{boost}",
                        staminaamount = boost
                    };
                    break;
                case 2:
                    int weaponType = Random.Next(3);
                    item = weaponType switch
                    {
                        0 => new Sword(),
                        1 => new Claymore(),
                        2 => new Axe(),
                        _ => new Food()
                    };
                    break;
                default:
                    item = new Food();
                    break;
            }

            if (item is Food food)
            {
                player.HP = Math.Min(player.HP + food.healamount, player.MaxHP);
                Console.WriteLine($"Использовано {food.name}, восстановлено {food.healamount} HP!");
            }
            else if (item is StaminaPotion potion)
            {
                player.Stamina = Math.Min(player.Stamina + potion.staminaamount, player.MaxStamina);
                Console.WriteLine($"Использовано {potion.name}, восстановлено {potion.staminaamount} выносливости!");
            }
            else if (item is Weapon newWeapon)
            {
                Console.WriteLine($"Найдено {newWeapon.name} (Урон={newWeapon.dmg}, Прочность={newWeapon.durability})");
                Console.WriteLine($"Текущее: {player.equippedweapon.name} (Урон={player.equippedweapon.dmg}, Прочность={player.equippedweapon.durability})");
                Console.WriteLine("Экипировать новое оружие? (Д/Н)");
                if (Console.ReadLine()?.ToUpper() == "Д")
                    player.equippedweapon = newWeapon;
            }
        }
    }
}