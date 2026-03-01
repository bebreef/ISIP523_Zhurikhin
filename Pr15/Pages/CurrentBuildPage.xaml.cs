using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Pr15;

namespace Pr15.Pages
{
    /// <summary>
    /// Логика взаимодействия для CurrentBuildPage.xaml
    /// </summary>
    public partial class CurrentBuildPage : Page
    {
        public CurrentBuildPage()
        {
            InitializeComponent();
            App.CurrentBuild.CollectionChanged += (s, e) => Refresh();
            Refresh();
        }

        private void Refresh()
        {
            lbBuild.Items.Refresh();
            tbTotal.Text = $"{App.CurrentBuild.Sum(p => p.price):N0} ₽";
            tbCompat.Text = GetCompatibilityMessage();
        }

        private string GetCompatibilityMessage()
        {
            var errs = new List<string>();

            var cpu = App.CurrentBuild.FirstOrDefault(p => p.parttype_.name == "CPU")?.cpu_;
            var mb = App.CurrentBuild.FirstOrDefault(p => p.parttype_.name == "Motherboard")?.motherboard_;
            var gpu = App.CurrentBuild.FirstOrDefault(p => p.parttype_.name == "GPU")?.gpu_;
            var psu = App.CurrentBuild.FirstOrDefault(p => p.parttype_.name == "PowerSupply")?.powersupply_;

            if (cpu != null && mb != null && cpu.socketid != mb.socketid)
                errs.Add("Сокет CPU и материнки не совпадает");

            if (mb != null && App.CurrentBuild.Any(p => p.parttype_.name == "RAM" && p.ram_.memorytypeid != mb.memorytypeid))
                errs.Add("Тип ОЗУ не подходит к материнке");

            if (gpu != null && psu != null && gpu.recommendpower.GetValueOrDefault(0) > psu.power)
                errs.Add($"БП слабый ({psu.power} Вт < {gpu.recommendpower} Вт)");

            return errs.Any() ? string.Join("\n", errs) : "Совместимо";
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is basepart_ p)
            {
                App.CurrentBuild.Remove(p);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ConfirmPage());
        }
    }
}
