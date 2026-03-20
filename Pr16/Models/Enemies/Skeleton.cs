using Pr16.Models.Entities;
using Pr16.Services;

namespace Pr16.Models.Enemies
{
    public class Skeleton : Enemy
    {
        public Skeleton()
        {
            Name = "Скелет";
            MaxHP = 40;
            HP = MaxHP;
            attack = 3;
            defense = 5;
        }

        public override void AttackPlayer(Player player)
        {
            int damage = attack;
            if (player.isDefending && Random.NextDouble() < 0.4)
            {
                LogAction?.Invoke($"{player.Name} уклоняется!");
                return;
            }
            player.HP -= damage;
            LogAction?.Invoke($"{Name} игнорирует броню и наносит {damage} урона");
        }
    }
}