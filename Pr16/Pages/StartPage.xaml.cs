using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Pr16.Models.Entities;
using Pr16.Models.Items;
using Pr16.ViewModels;

namespace Pr16.Pages
{
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();
            DataContext = new StartViewModel();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is StartViewModel vm)) return;

            Weapon weapon;
            if (vm.SelectedSword) weapon = new Sword();
            else if (vm.SelectedClaymore) weapon = new Claymore();
            else weapon = new Axe();

            Armor armor;
            if (vm.SelectedLightArmor) armor = new Armor { name = "Лёгкая броня", defense = 1 };
            else if (vm.SelectedMediumArmor) armor = new Armor { name = "Средняя броня", defense = 2 };
            else armor = new Armor { name = "Тяжёлая броня", defense = 4 };

            var player = new Player(
                string.IsNullOrWhiteSpace(vm.PlayerName) ? "Безымянный" : vm.PlayerName,
                30, weapon, armor);

            var gameVM = new GameViewModel();
            gameVM.Player = player;
            gameVM.Log.Add($"Добро пожаловать, {player.Name}!");
            gameVM.Log.Add($"Оружие: {weapon.name}   Броня: {armor.name}");

            var gamePage = new GamePage();
            gamePage.DataContext = gameVM;

            gameVM.NextTurn();  

            NavigationService?.Navigate(gamePage);
        }
    }
}