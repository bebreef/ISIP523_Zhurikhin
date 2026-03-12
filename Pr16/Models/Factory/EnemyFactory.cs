using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pr16.Models.Enemies;
using Pr16.Models.Entities;

namespace Pr16.Models.Factory
{
    public static class EnemyFactory
    {
        public static Enemy CreateRegular()
        {
            int r = Pr16.Services.Random.Next(4);
            switch (r)
            {
                case 0: return new Goblin();
                case 1: return new Skeleton();
                case 2: return new Mage();
                case 3: return new Slime();
                default: return new Slime(); 
            }
        }

        public static Enemy CreateBoss()
        {
            int r = Pr16.Services.Random.Next(4);
            switch (r)
            {
                case 0: return new VVG();
                case 1: return new Kovalsky();
                case 2: return new Archmage();
                case 3: return new Pestov();
                default: return new Pestov();
            }
        }
    }
}
