using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pr16.Models.Items;

namespace Pr16.Models.Entities
{
    public class Player : BaseEntity
    {
        public int Stamina;
        public int MaxStamina = 20;
        public Weapon equippedweapon;
        public Armor equippedarmor;
        public bool isFrozen;
        public bool isDefending;
        public List<object> Inventory = new List<object>();

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
            if (!isFrozen)
            {
                Stamina = Math.Min(Stamina + 1, MaxStamina);
            }
            isFrozen = false;
            Console.WriteLine($"Раунд боя завершён. +1 выносливость -> {Stamina}/{MaxStamina}");
        }

        public void RewardStamina()
        {
            Stamina = Math.Min(Stamina + 2, MaxStamina);
            Console.WriteLine($"Победа! Восстановлено +2 выносливости -> {Stamina}/{MaxStamina}");
        }
    }
}