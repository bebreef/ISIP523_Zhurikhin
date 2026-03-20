using System;

namespace Pr16.Models.Enemies
{
    public class VVG : Goblin
    {
        public VVG()
        {
            Name = "ВВГ";
            MaxHP = (int)Math.Round(30 * 2.0);
            HP = MaxHP;
            attack = (int)Math.Round(4 * 1.5);
            defense = (int)Math.Round(3 * 1.2);
            CritChance = 0.3;
        }
    }
}