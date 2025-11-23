using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP523_Zhurikhin
{
    public static class EnemyFactory
    {
        public static Enemy CreateRegular()
        {
            return Random.Next(4) switch
            {
                0 => new Goblin(),
                1 => new Skeleton(),
                2 => new Mage(),
                3 => new Slime()
            };
        }

        public static Enemy CreateBoss()
        {
            return Random.Next(4) switch
            {
                0 => new VVG(),
                1 => new Kovalsky(),
                2 => new Archmage(),
                3 => new Pestov()
            };
        }
    }
}