using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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

namespace Pr15.Pages
{
    /// <summary>
    /// Логика взаимодействия для CatalogPage.xaml
    /// </summary>
    public partial class CatalogPage : Page
    {
        private string searchText = "";
        private parttype_ selectedType;

        public CatalogPage()
        {
            InitializeComponent();
            cmbType.ItemsSource = App.PartTypes;
            cmbType.DisplayMemberPath = "name";
            LoadParts();
        }

        private void LoadParts()
        {
            var q = Core.Context.basepart_.AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
                q = q.Where(p => p.name.ToLower().Contains(searchText.ToLower()));

            if (selectedType != null)
                q = q.Where(p => p.parttypeid == selectedType.id);

            lbParts.ItemsSource = q.ToList();
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchText = tbSearch.Text.Trim();
            LoadParts();
        }

        private void cmbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedType = cmbType.SelectedItem as parttype_;
            LoadParts();
        }

        private void lbParts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbParts.SelectedItem is basepart_ newPart)
    {
                string typeName = newPart.parttype_.name ?? "";

                if (CanAddPart(typeName))
                {
                    App.CurrentBuild.Add(newPart);
                    MessageBox.Show($"Добавлено: {newPart.name}");
                    return;
                }

                string message = "";
                if (typeName == "RAM")
                    message = "Уже добавлено 4 плашки RAM. Заменить одну из них на новую?";
                else if (typeName == "CPU" || typeName == "Motherboard" || typeName == "Case" || typeName == "ProcessorCooler" || typeName == "PowerSupply" || typeName == "GPU")
                    message = $"Уже добавлен {typeName.ToLower()}. Заменить на новую модель?";
                else
                    message = "Лимит превышен. Заменить существующую деталь?";

                var result = MessageBox.Show(message, "Заменить деталь?",MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var toRemove = App.CurrentBuild.Where(p => p.parttype_.name == typeName).ToList();
                    foreach (var oldPart in toRemove)
                    {
                        App.CurrentBuild.Remove(oldPart);
                    }

                    App.CurrentBuild.Add(newPart);
                    MessageBox.Show($"Заменено на: {newPart.name}");
                }
            }
        }

        private bool CanAddPart(string typeName)
        {
            int count = 0;
            foreach (var p in App.CurrentBuild)
            {
                if (p.parttype_.name == typeName)
                    count++;
            }

            if (typeName == "CPU" || typeName == "Motherboard" || typeName == "Case" ||
                typeName == "ProcessorCooler" || typeName == "PowerSupply")
            {
                return count < 1;
            }
            else if (typeName == "GPU")
            {
                return count < 2;
            }
            else if (typeName == "RAM")
            {
                return count < 4;
            }

            return true;
        }
    }
}