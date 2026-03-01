using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Pr15
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ObservableCollection<basepart_> CurrentBuild { get; } = new ObservableCollection<basepart_>();

        public static List<parttype_> PartTypes { get; } = Core.Context.parttype_.ToList();
    }
}