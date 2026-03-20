using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pr16.Models.Items
{
    public class Weapon
    {
        public string name;
        public int basedmg;
        public int staminacost;
        public int durability = 20;
        public string ImagePath { get; set; }
        public string StatText => $"Урон: {basedmg}";
        public int dmg => durability > 0 ? basedmg : basedmg / 2;
    }

    public class Sword : Weapon
    {
        public Sword()
        {
            name = "Меч";
            basedmg = 4;
            staminacost = 1;
        }
    }

    public class Claymore : Weapon
    {
        public Claymore()
        {
            name = "Клеймор";
            basedmg = 6;
            staminacost = 2;
        }
    }

    public class Axe : Weapon
    {
        public Axe()
        {
            name = "Топор";
            basedmg = 8;
            staminacost = 3;
        }
    }
}