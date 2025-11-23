
class BaseEntity
{
    public int ID;
    public string Name;
    public int HP;
    public int MaxHP;
    public bool isAlive => HP > 0;
}
class Player : BaseEntity
{
    public int Stamina;
    public int MaxStamina = 15;
    public Weapon equippedweapon;
    public Armor equippedarmor;
    public bool isFrozen;
    public bool isDefending;
    public Player(string name, int health, Weapon startweapon, Armor startarmor)
    {
        Name = name;
        HP = health;
        MaxHP = health;
        Stamina = MaxStamina;
        equippedweapon = startweapon;
        equippedarmor = startarmor;
    }
    public void RegenerateStamina()
    {
        Stamina = Math.Min(Stamina + 2, MaxStamina); 
    }
}
class Weapon
{
    public string name;
    public int basedmg;
    public int staminacost;
    public int durability = 20;
    public int dmg => durability > 0 ? basedmg : basedmg / 2;
}
class Sword : Weapon
{
    public Sword()
    {
        name = "Меч";
        basedmg = 4;
        staminacost = 1;
    }
}
class Claymore : Weapon
{
    public Claymore()
    {
        name = "Клеймор";
        basedmg = 6;
        staminacost = 2;
    }
}
class Axe : Weapon
{
    public Axe()
    {
        name = "Топор";
        basedmg = 8;
        staminacost = 3;
    }
}
class Armor
{
    public string name;
    public int defense;
    public string bufftype;
}
class Food
{
    public string name = "ХЭЛБ";
    public int healamount=2;
}
class StaminaPotion
{
    public string name;
    public int staminaamount;
}
class Enemy : BaseEntity
{
    public int attack;
    public int defense;
    public virtual void AttackPlayer(Player player, Random rng)
    {
        int damage = attack;
        if (player.isDefending && rng.NextDouble() < 0.4)
        {
            Console.WriteLine($"{player.Name} уклоняется от атаки!");
        }
        else
        {
            damage = Math.Max(0, damage - player.equippedarmor.defense);
            player.HP -= damage;
            Console.WriteLine($"{Name} наносит {damage} урон(а) {player.Name}!");
        }
    }
}
class Goblin : Enemy
{
    public double CritChance;
    public Goblin()
    {
        Name = "Гоблин";
        MaxHP = 15; 
        HP = MaxHP;
        attack = 3; 
        defense = 1;
        CritChance = 0.2;
    }
    public override void AttackPlayer(Player player, Random rng)
    {
        int damage = attack;
        if (rng.NextDouble() < CritChance)
        {
            damage *= 2;
            Console.WriteLine($"{Name} наносит критический удар!"); 
        }
        if (player.isDefending && rng.NextDouble() < 0.4)
        {
            Console.WriteLine($"{player.Name} уклоняется от атаки!");
        }
        else
        {
            damage = Math.Max(0, damage - player.equippedarmor.defense);
            player.HP -= damage;
            Console.WriteLine($"{Name} наносит {damage} урон(а) {player.Name}!");
        }
    }
}
class Skeleton : Enemy
{
    public double FreezeChance; 
    public Skeleton()
    {
        Name = "Скелет";
        MaxHP = 20; 
        HP = MaxHP;
        attack = 3; 
        defense = 2; 
        FreezeChance = 0.0; 
    }
    public override void AttackPlayer(Player player, Random rng)
    {
        int damage = attack;
        if (player.isDefending && rng.NextDouble() < 0.4)
        {
            Console.WriteLine($"{player.Name} уклоняется от атаки!");
        }
        else
        {
            player.HP -= damage;
            Console.WriteLine($"{Name} игнорирует броню и наносит {damage} урона {player.Name}!");
        }
    }
}
class Mage : Enemy
{
    public double FreezeChance;
    public Mage()
    {
        Name = "Маг";
        MaxHP = 10;
        HP = MaxHP;
        attack = 2; 
        defense = 2; 
        FreezeChance = 0.15;
    }
    public override void AttackPlayer(Player player, Random rng)
    {
        if (rng.NextDouble() < FreezeChance)
        {
            player.isFrozen = true;
            Console.WriteLine($"{Name} замораживет {player.Name}! Пропуск хода!");
        }
        int damage = attack;    
        if (player.isDefending && rng.NextDouble() < 0.4)
        {
            Console.WriteLine($"{player.Name} уклоняется от атаки!");
        }
        else
        {
            damage = Math.Max(0, damage - player.equippedarmor.defense);
            player.HP -= damage;
            Console.WriteLine($"{Name} наносит {damage} урон(а) {player.Name}!");
        }
    }
}
class VVG : Goblin
{
    public VVG()
    {
        Name = "ВВГ";
        MaxHP = (int)Math.Round(15 * 2.0); 
        HP = MaxHP;
        attack = (int)Math.Round(3 * 1.5); 
        defense = (int)Math.Round(1 * 1.2);
        CritChance = 0.2 + 0.1;
    }
}
class Kovalsky : Skeleton
{
    public Kovalsky()
    {
        Name = "Ковальский";
        MaxHP = (int)Math.Round(20 * 2.5);
        HP = MaxHP;
        attack = (int)Math.Round(3 * 1.3); 
        defense = (int)Math.Round(2 * 1.4); 
        FreezeChance = 0.0; 
    }
}

class Archmage : Mage
{
    public Archmage()
    {
        Name = "Архимаг C++";
        MaxHP = (int)Math.Round(10 * 1.8); 
        HP = MaxHP;
        attack = (int)Math.Round(2 * 1.6); 
        defense = (int)Math.Round(2 * 1.1); 
        FreezeChance = 0.15 + 0.1; 
    }
}

class Pestov : Skeleton
{
    public Pestov()
    {
        Name = "Пестов C--";
        MaxHP = (int)Math.Round(20 * 1.3); 
        HP = MaxHP;
        attack = (int)Math.Round(3 * 1.8); 
        defense = (int)Math.Round(2 * 0.6); 
        FreezeChance = 0.15 + 0.15; 
    }

    public override void AttackPlayer(Player player, Random rng)
    {
        if (rng.NextDouble() < FreezeChance)
        {
            player.isFrozen = true;
            Console.WriteLine($"{Name} замораживает {player.Name}! Пропуск хода!");
        }
        int damage = attack;
        if (player.isDefending && rng.NextDouble() < 0.4)
        {
            Console.WriteLine($"{player.Name} уклоняется от атаки!");
        }
        else
        {
            player.HP -= damage;
            Console.WriteLine($"{Name} игнорирует броню и наносит {damage} урона {player.Name}!");
        }
    }
}
class Game
{
    private Player player;
    private Random rng = new Random();
    private int turnCount = 0;
    public void start()
    {
        Console.WriteLine("Как вас будут звать?");
        string name = Console.ReadLine();
        Console.WriteLine("Выберите стартовое оружие. (1-Меч, 2-Клеймор, 3-Топор)");
        string choice = Console.ReadLine();
        Weapon startWeapon = choice switch
        {
            "1" => new Sword(),
            "2" => new Claymore(),
            "3" => new Axe(),
        };
        Console.WriteLine("Выберите броню. (1-Тяжелая Броня (+1 урон Меча), 2-Средняя Броня (+1 урон Клеймора), 3-Легкая Броня (+1 урон Топора))");
        string armorChoice = Console.ReadLine();
        Armor startArmor = armorChoice switch
        {
            "1" => new Armor { name = "Тяжелая Броня", defense = 1, bufftype = "Меч" },
            "2" => new Armor { name = "Средняя Броня", defense = 1, bufftype = "Клеймор" },
            "3" => new Armor { name = "Легкая Броня", defense = 1, bufftype = "Топор" },
        };
        player = new Player(name, 20, startWeapon, startArmor);
        RunGame();
    
    }
    public void RunGame()
    {
        while (player.isAlive)
        {
            turnCount++;
            Console.WriteLine($"\nХод {turnCount}");
            DisplayStats();
            if (turnCount % 5 == 0)
            {
                FightBoss();
            }
            else if (rng.NextDouble() < 0.3)
            {
                OpenChest();
            }
            else
            {
                FightEnemy();
            }
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
        {
            Enemy enemy = rng.Next(3) switch
            {
                0 => new Goblin(),
                1 => new Skeleton(),
                2 => new Mage()
            };
            Fight(player, enemy);
        }

    }
    private void FightBoss()
    {
        Enemy boss = rng.Next(4) switch
        {
            0 => new VVG(),
            1 => new Kovalsky(),
            2 => new Archmage(),
            3 => new Pestov()
        };
        Fight(player, boss);
    }
    private void Fight(Player player, Enemy enemy)
    {
        Console.WriteLine($"{enemy.Name} появляется!");
        while (player.isAlive && enemy.isAlive)
        {
            Console.WriteLine($"{enemy.Name}: HP={enemy.HP}/{enemy.MaxHP}");
            if (!player.isFrozen)
            {
                Console.WriteLine("Выберите: 1-Аттаковать, 2-Защищаться");
                string choice = Console.ReadLine().ToUpper();
                player.isDefending = false;
                if (choice == "1" && player.Stamina >= player.equippedweapon.staminacost)
                {
                    player.Stamina -= player.equippedweapon.staminacost;
                    player.equippedweapon.durability = Math.Max(1, player.equippedweapon.durability - 1);
                    int damage = player.equippedweapon.dmg;
                    if (player.equippedarmor.bufftype == player.equippedweapon.name)
                        damage += 1; 
                    enemy.HP -= damage;
                    Console.WriteLine($"{player.Name} атакует с помощью {player.equippedweapon.name} и наносит {damage} урона!");
                    Console.WriteLine($"-------------------------------------");
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
                enemy.AttackPlayer(player, rng);
            if (!enemy.isAlive)
                OpenChest();
        }

    }
    private void OpenChest()
    {
        Console.WriteLine("Вы нашли сундук!");
        object item;
        int chestReward = rng.Next(3);
        switch (chestReward)
        {
            case 0:
                item = new Food();
                break;
            case 1:
                int boost = rng.Next(1, 4);
                item = new StaminaPotion
                {
                    name = $"Зелье выносливости +{boost}",
                    staminaamount = boost
                };
                break;
            case 2:
                int weaponType = rng.Next(3);
                switch (weaponType)
                {
                    case 0:
                        item = new Sword();
                        break;
                    case 1:
                        item = new Claymore();
                        break;
                    case 2:
                        item = new Axe();
                        break;
                    default:
                        item = new Food();
                        break;
                }
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
            if (Console.ReadLine().ToUpper() == "Д")
                player.equippedweapon = newWeapon;
        }
    }
}

class Program
{
    static void Main()
    {
        new Game().start();
    }
}

