using Pr16.Models.Entities;
using Pr16.Services;
using System;

namespace Pr16.Models.Enemies
{
    public class Pestov : Skeleton
    {
        public double FreezeChance = 0.15;

        public Pestov()
        {
            Name = "Пестов C--";
            MaxHP = (int)Math.Round(40 * 1.3);
            HP = MaxHP;
            attack = (int)Math.Round(3 * 1.8);
            defense = (int)Math.Round(5 * 0.6);
            FreezeChance = 0.30;
        }

        public override void AttackPlayer(Player player)
        {
            if (Pr16.Services.Random.NextDouble() < FreezeChance)
            {
                player.isFrozen = true;
                LogAction?.Invoke($"{Name} замораживает {player.Name}! Пропуск хода!");
            }
            int damage = attack;
            if (player.isDefending && Pr16.Services.Random.NextDouble() < 0.4)
            {
                LogAction?.Invoke($"{player.Name} уклоняется!");
                return;
            }
            player.HP -= damage;
            LogAction?.Invoke($"{Name} игнорирует броню и наносит {damage} урона!");
        }
    }
}