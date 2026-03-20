using Pr16.Models.Entities;
using Pr16.Services;

namespace Pr16.Models.Enemies
{
    public class Goblin : Enemy
    {
        public double CritChance = 0.2;

        public Goblin()
        {
            Name = "Гоблин";
            MaxHP = 30;
            HP = MaxHP;
            attack = 4;
            defense = 3;
        }

        public override void AttackPlayer(Player player)
        {
            int damage = attack;
            if (Random.NextDouble() < CritChance)
            {
                damage *= 2;
                LogAction?.Invoke($"{Name} наносит КРИТ!");
            }
            base.AttackPlayer(player);   
        }
    }
}