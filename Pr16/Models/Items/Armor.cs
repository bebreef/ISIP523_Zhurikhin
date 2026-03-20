using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pr16.Models.Items
{
    public class Armor
    {
        public string name;
        public int defense;
        public string bufftype;
        public string ImagePath { get; set; }
        public string StatText => $"Защита: {defense}";
        public Armor(string name, int defense, string imagePath)
        {
            this.name = name;
            this.defense = defense;
            this.ImagePath = imagePath;
        }
    }
}
