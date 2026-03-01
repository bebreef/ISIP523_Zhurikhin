using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
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

namespace Pr15.Pages
{
    /// <summary>
    /// Логика взаимодействия для ConfirmPage.xaml
    /// </summary>
    public partial class ConfirmPage : Page
    {
        public ConfirmPage()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string name = tbName.Text.Trim();
            string author = tbAuthor.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите название сборки");
                return;
            }

            var asm = new assembly_ { name = name, author = author };
            Core.Context.assembly_.Add(asm);
            Core.Context.SaveChanges();

            foreach (var p in App.CurrentBuild)
            {
                Core.Context.partassembly_.Add(new partassembly_ { assemblyid = asm.id, partid = p.id });
            }
            Core.Context.SaveChanges();

            App.CurrentBuild.Clear();
            MessageBox.Show("Сборка сохранена!");
            NavigationService?.Navigate(new SavedBuildsPage());
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}