using Pr16.Models.Entities;
using Pr16.Services;
using System;

namespace Pr16.Models.Enemies
{
    public class Slime : Enemy
    {
        public Slime()
        {
            Name = "Слизень";
            MaxHP = 25;
            HP = MaxHP;
            attack = 2;
            defense = 0;
        }

        public override void TakeDamage(int damage)
        {
            int reduced = Math.Max(0, damage - 2);

            if (damage > reduced)
                LogAction?.Invoke("Слизень поглощает 2 урона!");

            HP -= reduced;
            LogAction?.Invoke($"{Name} получает {reduced} урона");
        }

        public override void AttackPlayer(Player player)
        {
            int damage = attack;
            if (player.isDefending)
            {
                if (Pr16.Services.Random.NextDouble() < 0.4)
                {
                    LogAction?.Invoke($"{player.Name} уклоняется!");
                    return;
                }
                double blockPercent = Pr16.Services.Random.Next(70, 101) / 100.0;
                int block = (int)(player.equippedarmor.defense * blockPercent);
                damage = Math.Max(0, damage - block);
                LogAction?.Invoke($"{player.Name} блокирует {block} урона");
            }
            else
            {
                damage = Math.Max(0, damage - player.equippedarmor.defense);
            }
            player.HP -= damage;
            LogAction?.Invoke($"{Name} наносит {damage} урона {player.Name}");
        }
    }
}