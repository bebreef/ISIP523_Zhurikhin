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
            if (vm.SelectedSword) weapon = new Sword() { ImagePath = "/Assets/sword.png" };
            else if (vm.SelectedClaymore) weapon = new Claymore() { ImagePath = "/Assets/claymore.png" };
            else weapon = new Axe() { ImagePath = "/Assets/axe.png" };

            Armor armor;
            if (vm.SelectedLightArmor) armor = new Armor("Лёгкая броня", 1, "/Assets/lightarmor.png");
            else if (vm.SelectedMediumArmor) armor = new Armor("Средняя броня", 2, "/Assets/mediumarmor.png");
            else armor = new Armor("Тяжёлая броня", 4, "/Assets/heavyarmor.png");

            var player = new Player(
                string.IsNullOrWhiteSpace(vm.PlayerName) ? "Безымянный" : vm.PlayerName,
                100, weapon, armor);
            player.Inventory.Add(weapon);
            player.Inventory.Add(armor);

            var gameVM = new GameViewModel();
            gameVM.Player = player;


            gameVM.LogAdd($"Добро пожаловать, {player.Name}!");
            gameVM.LogAdd($"Оружие: {weapon.name} Броня: {armor.name}");

            var gamePage = new GamePage();
            gamePage.DataContext = gameVM;
            gameVM.NextTurn();

            NavigationService?.Navigate(gamePage);
        }
    }
}