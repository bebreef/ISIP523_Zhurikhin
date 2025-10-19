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
    public weapon equippedweapon;
    public armor equippedarmor;
    public bool isFrozen;
    public bool isDefending;
    public Player(string name, int health, weapon startweapon, armor startarmor)
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
            damage = Math.Max(0, damage - player.equippedarmor.Defense);
            player.HP -= damage;
            Console.WriteLine($"{Name} наносит {damage} урон(а) {player.Name}!");
        }
    }
}
class Goblin : Enemy
{
    public double CritChance = 0.2;
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
            damage = Math.Max(0, damage - player.equippedarmor.Defense);
            player.HP -= damage;
            Console.WriteLine($"{Name} наносит {damage} урон(а) {player.Name}!");
        }
    }
}
class Skeleton : Enemy
{
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
            Console.WriteLine($"{Name} наносит {damage} урон(а) {player.Name}!");
        }
    }
}
class Mage : Enemy
{
    public double FreezeChance = 0.15;
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
            damage = Math.Max(0, damage - player.equippedarmor.Defense);
            player.HP -= damage;
            Console.WriteLine($"{Name} наносит {damage} урон(а) {player.Name}!");
        }
    }
}

