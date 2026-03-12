using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pr16.Models.Entities
{
    public class BaseEntity
    {
        public int ID;
        public string Name = "";
        public int HP;
        public int MaxHP;
        public bool isAlive => HP > 0;
    }
}