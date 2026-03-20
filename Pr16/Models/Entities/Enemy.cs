using Pr16.Models.Entities;
using Pr16.Services;
using System;

namespace Pr16.Models.Enemies
{
    public abstract class Enemy : BaseEntity
    {
        public int attack;
        public int defense;

        public Action<string> LogAction;

        public virtual void TakeDamage(int damage)
        {
            HP -= damage;
        }

        public virtual void AttackPlayer(Player player)
        {
            int damage = attack;
            if (player.isDefending && Pr16.Services.Random.NextDouble() < 0.4)
            {
                LogAction?.Invoke($"{player.Name} уклоняется от атаки!");
                return;
            }
            if (player.isDefending)
            {
                double blockPercent = Pr16.Services.Random.NextDouble() * 0.3 + 0.7;
                int blocked = (int)(player.equippedarmor.defense * blockPercent);
                damage = Math.Max(0, damage - blocked);
            }
            else
            {
                damage = Math.Max(0, damage - player.equippedarmor.defense);
            }
            player.HP -= damage;
            LogAction?.Invoke($"{Name} наносит {damage} урона {player.Name}!");
        }
    }
}