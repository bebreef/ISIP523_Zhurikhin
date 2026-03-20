using System;
using Pr16.Models.Entities;
using Pr16.Services;

namespace Pr16.Models.Enemies
{
    public class Mage : Enemy
    {
        public double FreezeChance = 0.15;

        public Mage()
        {
            Name = "Маг";
            MaxHP = 25;
            HP = MaxHP;
            attack = 5;
            defense = 2;
        }

        public override void AttackPlayer(Player player)
        {
            if (Pr16.Services.Random.NextDouble() < FreezeChance)
            {
                player.isFrozen = true;
                LogAction?.Invoke($"{Name} замораживает {player.Name}! Пропуск хода!");
            }
            base.AttackPlayer(player);
        }
    }
}