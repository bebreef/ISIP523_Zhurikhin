using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP523_Zhurikhin
{
    public class Mage : Enemy
    {
        public double FreezeChance = 0.15;

        public Mage()
        {
            Name = "Маг";
            MaxHP = 10;
            HP = MaxHP;
            attack = 2;
            defense = 2;
        }

        public override void AttackPlayer(Player player)
        {
            if (Random.NextDouble() < FreezeChance)
            {
                player.isFrozen = true;
                Console.WriteLine($"{Name} замораживет {player.Name}! Пропуск хода!");
            }
            int damage = attack;
            if (player.isDefending && Random.NextDouble() < 0.4)
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
}
