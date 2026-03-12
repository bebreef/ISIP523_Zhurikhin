using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pr16.Models.Entities
{
    public abstract class Enemy : BaseEntity
    {
        public int attack;
        public int defense;
        public virtual void TakeDamage(int damage)
        {
            HP -= damage;
        }

        public virtual void AttackPlayer(Player player)
        {
            int damage = attack;

            if (player.isDefending && Pr16.Services.Random.NextDouble() < 0.4)
            {
                Console.WriteLine($"{player.Name} уклоняется от атаки!");
                return;
            }

            damage = Math.Max(0, damage - player.equippedarmor.defense);
            player.HP -= damage;
            Console.WriteLine($"{Name} наносит {damage} урон(а) {player.Name}!");
        }
    }
}