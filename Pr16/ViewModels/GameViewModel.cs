using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Pr16.Models.Entities;
using Pr16.Models.Factory;
using Pr16.Models.Items;
using System.Windows.Input;
using System.Windows;

namespace Pr16.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        public Player Player { get; private set; }
        public ObservableCollection<string> Log { get; } = new ObservableCollection<string>();

        private object _currentEncounter;
        public object CurrentEncounter
        {
            get => _currentEncounter;
            set
            {
                _currentEncounter = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FightingControlsVisibility));
                OnPropertyChanged(nameof(ChestControlsVisibility));
                OnPropertyChanged(nameof(EncounterImagePath));
            }
        }

        public Visibility FightingControlsVisibility => CurrentEncounter is Enemy ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        public Visibility ChestControlsVisibility => CurrentEncounter is ChestState ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

        public string EncounterImagePath
        {
            get
            {
                if (CurrentEncounter is Enemy enemy)
                {
                    string n = enemy.Name;
                    if (n == "Гоблин" || n == "ВВГ") return "/Assets/goblin.png";
                    if (n == "Скелет" || n == "Ковальский" || n == "Пестов C--") return "/Assets/skeleton.png";
                    if (n == "Маг" || n == "Архимаг C++") return "/Assets/mage.png";
                    if (n == "Слизень") return "/Assets/slime.png";
                    return "/Assets/unknown.png";
                }
                if (CurrentEncounter is ChestState) return "/Assets/chest.png";
                return "/Assets/dungeon_empty.png";
            }
        }

        public int TurnCount { get; private set; }

        public class ChestState { public object Item { get; set; } }

        private bool _isDefending;
        private readonly Random _rng = new Random();

        public GameViewModel() { }

        public void StartNewGame()
        {
            Log.Clear();
            TurnCount = 0;
            Log.Add("Добро пожаловать в подземелье!");

            Player = new Player("Герой", 20, new Sword(), new Armor { name = "Тяжелая Броня", defense = 1, bufftype = "Меч" });

            Log.Add("Вы спустились на первый уровень...");
            NextTurn();
        }

        public void NextTurn()
        {
            if (Player?.HP <= 0) return;

            TurnCount++;
            Log.Add($"\n=== Ход {TurnCount} ===");

            if (TurnCount % 10 == 0)
            {
                var boss = EnemyFactory.CreateBoss();
                CurrentEncounter = boss;
                Log.Add($"БОСС {boss.Name} появляется!");
            }
            else if (_rng.NextDouble() < 0.5)
            {
                CurrentEncounter = new ChestState();
                Log.Add("Вы нашли сундук!");
            }
            else
            {
                var enemy = EnemyFactory.CreateRegular();
                CurrentEncounter = enemy;
                Log.Add($"{enemy.Name} появляется!");
            }

            OnPropertyChanged(nameof(TurnCount));
        }

        public void PerformAttack()
        {
            var enemy = CurrentEncounter as Enemy;
            if (enemy == null) return;

            Log.Add($"{Player.Name} готовится к атаке...");

            if (Player.Stamina < Player.equippedweapon.staminacost)
            {
                Log.Add("Недостаточно выносливости!");
            }
            else
            {
                Player.Stamina -= Player.equippedweapon.staminacost;
                Player.equippedweapon.durability = Math.Max(1, Player.equippedweapon.durability - 1);

                int damage = Player.equippedweapon.dmg;
                if (Player.equippedarmor.bufftype == Player.equippedweapon.name) damage += 1;

                enemy.TakeDamage(damage);
                Log.Add($"Вы нанесли {damage} урона {enemy.Name}! (HP врага: {enemy.HP}/{enemy.MaxHP})");
            }

            EnemyTurn(enemy);
            CheckFightEnd(enemy);
        }

        public void PerformDefend()
        {
            _isDefending = true;
            Log.Add($"{Player.Name} занимает оборонительную стойку!");
            EnemyTurn((Enemy)CurrentEncounter);
            _isDefending = false;
            CheckFightEnd((Enemy)CurrentEncounter);
        }

        private void EnemyTurn(Enemy enemy)
        {
            if (!enemy.isAlive) return;
            enemy.AttackPlayer(Player);
            Player.RegenerateStamina();

            if (Player.HP <= 0)
            {
                Log.Add("ВЫ ПОГИБЛИ...");
                CurrentEncounter = null;
            }

            OnPropertyChanged(nameof(Player));
        }

        private void CheckFightEnd(Enemy enemy)
        {
            if (!enemy.isAlive)
            {
                Log.Add($"{enemy.Name} повержен!");
                Player.RewardStamina();
                CurrentEncounter = null;
                NextTurn();
            }
        }

        public void EquipItem()
        {
            if (CurrentEncounter is ChestState state && state.Item is Weapon w)
            {
                Player.equippedweapon = w;
                Log.Add($"Вы экипировали {w.name}!");
            }
            CurrentEncounter = null;
            NextTurn();
        }

        public void SkipItem()
        {
            Log.Add("Вы оставили предмет лежать в сундуке.");
            CurrentEncounter = null;
            NextTurn();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}