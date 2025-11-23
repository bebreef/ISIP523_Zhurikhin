using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP523_Zhurikhin
{
    public class Player : BaseEntity
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
}