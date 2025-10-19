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
    public int MaxStamina = 10;
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
    public int durability = 10;
    public int dmg => Math.Max(1, basedmg * durability / 10);
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
        name = "Клеймор";
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
    public string name;
    public int healamount=10;
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
        MaxHP = 40;
        HP = MaxHP;
        attack = 10;
        defense = 3;
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
        Name = "Skeleton";
        MaxHP = 60;
        HP = MaxHP;
        attack = 8;
        defense = 5;
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
        Name = "Mage";
        MaxHP = 30;
        HP = MaxHP;
        attack = 6;
        defense = 7;
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
        MaxHP = (int)Math.Round(40 * 2.0); 
        HP = MaxHP;
        attack = (int)Math.Round(10 * 1.5); 
        defense = (int)Math.Round(3 * 1.2); 
        CritChance = 0.2 + 0.1;
    }
}
class Kovalsky : Skeleton
{
    public Kovalsky()
    {
        Name = "Ковальский";
        MaxHP = (int)Math.Round(60 * 2.5); 
        HP = MaxHP;
        attack = (int)Math.Round(8 * 1.3); 
        defense = (int)Math.Round(5 * 1.4);
        FreezeChance = 0.0; 
    }
}

class Archmage : Mage
{
    public Archmage()
    {
        Name = "Архимаг C++";
        MaxHP = (int)Math.Round(30 * 1.8); 
        HP = MaxHP;
        attack = (int)Math.Round(6 * 1.6); 
        defense = (int)Math.Round(7 * 1.1); 
        FreezeChance = 0.15 + 0.1; 
    }
}

class Pestov : Skeleton
{
    public Pestov()
    {
        Name = "Пестов C--";
        MaxHP = (int)Math.Round(60 * 1.3); 
        HP = MaxHP;
        attack = (int)Math.Round(8 * 1.8); 
        defense = (int)Math.Round(5 * 0.6);
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

}
