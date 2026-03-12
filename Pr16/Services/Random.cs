using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pr16.Services
{
    public static class Random
    {
        private static readonly System.Random rng = new System.Random();

        public static int Next(int max) => rng.Next(max);
        public static int Next(int min, int max) => rng.Next(min, max);
        public static double NextDouble() => rng.NextDouble();
    }
}