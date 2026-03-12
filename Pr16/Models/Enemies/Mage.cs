using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pr16.Models.Entities;
using System.Xml.Linq;

namespace Pr16.Models.Enemies
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
            if (Pr16.Services.Random.NextDouble() < FreezeChance)
            {
                player.isFrozen = true;
                Console.WriteLine($"{Name} замораживет {player.Name}! Пропуск хода!");
            }
            int damage = attack;
            if (player.isDefending && Pr16.Services.Random.NextDouble() < 0.4)
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