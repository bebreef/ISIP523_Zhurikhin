using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pr16.Models.Entities;
using System.Xml.Linq;

namespace Pr16.Models.Enemies
{
    public class Skeleton : Enemy
    {
        public Skeleton()
        {
            Name = "Скелет";
            MaxHP = 20;
            HP = MaxHP;
            attack = 3;
            defense = 2;
        }

        public override void AttackPlayer(Player player)
        {
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