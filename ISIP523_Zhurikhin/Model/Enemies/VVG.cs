using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP523_Zhurikhin
{
    public class VVG : Goblin
    {
        public VVG()
        {
            Name = "ВВГ";
            MaxHP = (int)Math.Round(15 * 2.0);
            HP = MaxHP;
            attack = (int)Math.Round(3 * 1.5);
            defense = (int)Math.Round(1 * 1.2);
            CritChance = 0.2 + 0.1;
        }
    }
}
