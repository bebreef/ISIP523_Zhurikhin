using System;

namespace Pr16.Models.Enemies
{
    public class Kovalsky : Skeleton
    {
        public Kovalsky()
        {
            Name = "Ковальский";
            MaxHP = (int)Math.Round(40 * 2.5);
            HP = MaxHP;
            attack = (int)Math.Round(3 * 1.3);
            defense = (int)Math.Round(5 * 1.4);
        }
    }
}