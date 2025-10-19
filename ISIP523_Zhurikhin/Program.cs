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


