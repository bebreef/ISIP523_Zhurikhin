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

namespace Pr15.Pages
{
    /// <summary>
    /// Логика взаимодействия для SavedBuildsPage.xaml
    /// </summary>
    public partial class SavedBuildsPage : Page
    {
        public SavedBuildsPage()
        {
            InitializeComponent();
            LoadSaved();
        }

        private void LoadSaved()
        {
            lbSaved.ItemsSource = Core.Context.assembly_.ToList();
            lbSaved.DisplayMemberPath = "name";
        }

        private void lbSaved_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbSaved.SelectedItem is assembly_ asm)
            {
                string info = $"Название: {asm.name}\nАвтор: {asm.author}\nДеталей: {asm.partassembly_.Count}\nЦена: {asm.partassembly_.Sum(pa => pa.basepart_.price):N0} ₽";
                MessageBox.Show(info);
            }
        }
    }
}