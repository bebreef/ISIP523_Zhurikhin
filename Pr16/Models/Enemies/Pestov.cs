using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pr16.Models.Entities;
using System.Xml.Linq;

namespace Pr16.Models.Enemies
{
    public class Pestov : Skeleton
    {
        public double FreezeChance = 0.15 + 0.15;

        public Pestov()
        {
            Name = "Пестов C--";
            MaxHP = (int)Math.Round(20 * 1.3);
            HP = MaxHP;
            attack = (int)Math.Round(3 * 1.8);
            defense = (int)Math.Round(2 * 0.6);
        }

        public override void AttackPlayer(Player player)
        {
            if (Pr16.Services.Random.NextDouble() < FreezeChance)
            {
                player.isFrozen = true;
                Console.WriteLine($"{Name} замораживает {player.Name}! Пропуск хода!");
            }
            int damage = attack;
            if (player.isDefending && Pr16.Services.Random.NextDouble() < 0.4)
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
}