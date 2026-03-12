using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Pr16.Models.Enemies
{
    public class Archmage : Mage
    {
        public Archmage()
        {
            Name = "Архимаг C++";
            MaxHP = (int)Math.Round(10 * 1.8);
            HP = MaxHP;
            attack = (int)Math.Round(2 * 1.6);
            defense = (int)Math.Round(2 * 1.1);
            FreezeChance = 0.15 + 0.1;
        }
    }
}