using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pr16.Models.Entities;
using System.Xml.Linq;

namespace Pr16.Models.Enemies
{
    public class Goblin : Enemy
    {
        public double CritChance = 0.2;

        public Goblin()
        {
            Name = "Гоблин";
            MaxHP = 15;
            HP = MaxHP;
            attack = 3;
            defense = 1;
        }

        public override void AttackPlayer(Player player)
        {
            int damage = attack;
            if (Pr16.Services.Random.NextDouble() < CritChance)
            {
                damage *= 2;
                Console.WriteLine($"{Name} наносит критический удар!");
            }
            if (player.isDefending && Pr16.Services.Random.NextDouble() < 0.4)
                Console.WriteLine($"{player.Name} уклоняется от атаки!");
            else
            {
                damage = Math.Max(0, damage - player.equippedarmor.defense);
                player.HP -= damage;
                Console.WriteLine($"{Name} наносит {damage} урон(а) {player.Name}!");
            }
        }
    }
}