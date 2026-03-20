using System.Collections.ObjectModel;
using Pr16.Models.Items;

namespace Pr16.Models.Entities
{
    public class Player : BaseEntity
    {
        public int Stamina;
        public int MaxStamina = 30;

        public Weapon equippedweapon;
        public Armor equippedarmor;

        public bool isFrozen;
        public bool isDefending;

        public ObservableCollection<object> Inventory { get; } = new ObservableCollection<object>();

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
                Stamina = System.Math.Min(Stamina + 1, MaxStamina);

            isFrozen = false;
        }

        public void RewardStamina()
        {
            Stamina = System.Math.Min(Stamina + 2, MaxStamina);
        }
        public bool TrySpendStamina(int amount)
        {
            if (Stamina < amount)
                return false;

            Stamina -= amount;
            return true;
        }

        public void ResetTurnStates()
        {
            isDefending = false;
        }
    }
}