using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using UPShootAndBunny;

namespace UPShootAndBunny
{
    public partial class App : Application
    {
        public static Users CurrentUser { get; set; }
    }
}