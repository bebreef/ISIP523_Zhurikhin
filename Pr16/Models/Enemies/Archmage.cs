using System;

namespace Pr16.Models.Enemies
{
    public class Archmage : Mage
    {
        public Archmage()
        {
            Name = "Архимаг C++";
            MaxHP = (int)Math.Round(25 * 1.8);
            HP = MaxHP;
            attack = (int)Math.Round(5 * 1.6);
            defense = (int)Math.Round(2 * 1.1);
            FreezeChance = 0.25; 
        }
    }
}