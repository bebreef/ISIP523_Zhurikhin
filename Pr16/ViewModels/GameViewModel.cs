using System;
using System.ComponentModel;
using Pr16.Models.Entities;
using Pr16.Models.Factory;
using Pr16.Models.Items;
using Pr16.Services;
using Pr16.Models.Enemies;
using Pr16.Pages;
using System.Windows;

namespace Pr16.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        public Player Player { get; set; }
        public object CurrentEncounter { get; set; }
        public object ChestItem { get; set; }
        public int TurnCount { get; private set; }
        public bool AfterBattleChest { get; set; }

        private string _logText = "";
        public string LogText
        {
            get => _logText;
            set
            {
                _logText = value;
                OnPropertyChanged(nameof(LogText));
            }
        }

        public void LogAdd(string message)
        {
            if (string.IsNullOrEmpty(LogText))
                LogText = message;
            else
                LogText += Environment.NewLine + message;
        }

        private int bossDefeatedCount;
        private string[] backgroundImages;

        public string HpText => $"{Player.HP}/{Player.MaxHP}";
        public string StaminaText => $"{Player.Stamina}/{Player.MaxStamina}";

        public string EncounterImagePath
        {
            get
            {
                if (CurrentEncounter is ChestState) return "/Assets/chest.png";
                Enemy enemy = CurrentEncounter as Enemy;
                if (enemy != null)
                {
                    string name = enemy.Name.ToLowerInvariant();
                    if (name.Contains("гоблин") || name.Contains("ввг")) return "/Assets/goblin.png";
                    if (name.Contains("скелет") || name.Contains("ковальский") || name.Contains("пестов")) return "/Assets/skeleton.png";
                    if (name.Contains("маг") || name.Contains("архимаг")) return "/Assets/mage.png";
                    if (name.Contains("слизень")) return "/Assets/slime.png";
                }
                return "/Assets/unknown_enemy.png";
            }
        }

        public string BackgroundImagePath
        {
            get
            {
                if (backgroundImages == null || backgroundImages.Length == 0) return "/Assets/background_default.png";
                return backgroundImages[bossDefeatedCount % backgroundImages.Length];
            }
        }

        public Visibility FightingControlsVisibility
        {
            get { return (CurrentEncounter is Enemy) ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility ChestControlsVisibility
        {
            get { return (CurrentEncounter is ChestState) ? Visibility.Visible : Visibility.Collapsed; }
        }

        public string ChestAction1Text { get; set; } = "Взять";
        public string ChestAction2Text { get; set; } = "Оставить";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public GameViewModel()
        {
            LogAdd("Добро пожаловать в подземелье...");
            bossDefeatedCount = 0;
            backgroundImages = new string[]
            {
                "/Assets/background_1.png",
                "/Assets/background_2.png",
                "/Assets/background_3.png"
            };
        }

        public void NextTurn()
        {
            if (Player == null || Player.HP <= 0) return;
            TurnCount++;
            LogAdd("");
            LogAdd($"──── Ход {TurnCount} ────");
            if (TurnCount % 5 == 0)
            {
                Enemy boss = EnemyFactory.CreateBoss();
                CurrentEncounter = boss;
                LogAdd($"!!! ПОЯВИЛСЯ БОСС: {boss.Name} !!!");
                OnPropertyChanged(nameof(CurrentEncounter));
                OnPropertyChanged(nameof(EncounterImagePath));
                OnPropertyChanged(nameof(BackgroundImagePath));
                OnPropertyChanged(nameof(FightingControlsVisibility));
                OnPropertyChanged(nameof(ChestControlsVisibility));
                return;
            }
            if (AfterBattleChest)
            {
                SpawnChest();
                AfterBattleChest = false;
                return;
            }
            double chance = Pr16.Services.Random.NextDouble();
            if (chance < 0.5)
            {
                SpawnChest();
            }
            else
            {
                SpawnEnemy();
            }
        }

        private void SpawnEnemy()
        {
            Enemy enemy = EnemyFactory.CreateRegular();
            enemy.LogAction = (msg) => LogAdd(msg);
            CurrentEncounter = enemy;
            LogAdd($"Появился враг: {enemy.Name} (HP: {enemy.HP})");
            OnPropertyChanged(nameof(CurrentEncounter));
            OnPropertyChanged(nameof(EncounterImagePath));
            OnPropertyChanged(nameof(FightingControlsVisibility));
            OnPropertyChanged(nameof(ChestControlsVisibility));
        }

        private void SpawnChest()
        {
            ChestItem = GenerateRandomItem();
            CurrentEncounter = new ChestState { Item = ChestItem };
            LogAdd("Вы нашли сундук!");
            LogAdd($"Внутри: {GetItemName(ChestItem)}");
            OnPropertyChanged(nameof(CurrentEncounter));
            OnPropertyChanged(nameof(EncounterImagePath));
            OnPropertyChanged(nameof(FightingControlsVisibility));
            OnPropertyChanged(nameof(ChestControlsVisibility));
            UpdateChestButtons();
        }

        private object GenerateRandomItem()
        {
            int r = Pr16.Services.Random.Next(6);
            if (r == 0) return new Sword();
            if (r == 1) return new Claymore();
            if (r == 2) return new Axe();
            if (r == 3) return new Food();
            if (r == 4) return new StaminaPotion { name = "Зелье выносливости", staminaamount = 8 };
            return new Armor { name = "Железная броня", defense = 3 };
        }

        private string GetItemName(object item)
        {
            if (item is Weapon w) return $"{w.name} (урон {w.basedmg})";
            if (item is Armor a) return $"{a.name} (защита {a.defense})";
            if (item is Food f) return $"Еда ({f.healamount} HP)";
            if (item is StaminaPotion s) return $"Зелье выносливости (+{s.staminaamount})";
            return "Неизвестный предмет";
        }

        private void UpdateChestButtons()
        {
            bool isConsumable = ChestItem is Food || ChestItem is StaminaPotion;
            ChestAction1Text = isConsumable ? "Использовать" : "Взять";
            ChestAction2Text = isConsumable ? "В инвентарь" : "Оставить";
            OnPropertyChanged(nameof(ChestAction1Text));
            OnPropertyChanged(nameof(ChestAction2Text));
        }

        private void UpdateUI()
        {
            OnPropertyChanged(nameof(Player));
            OnPropertyChanged(nameof(HpText));
            OnPropertyChanged(nameof(StaminaText));
        }

        public void Attack()
        {
            if (!(CurrentEncounter is Enemy enemy)) return;
            if (Player.isFrozen)
            {
                LogAdd($"{Player.Name} заморожен и пропускает ход!");
                Player.isFrozen = false;
                enemy.AttackPlayer(Player);
                Player.RegenerateStamina();
                UpdateUI();
                return;
            }
            int cost = Player.equippedweapon.staminacost;
            if (!Player.TrySpendStamina(cost))
            {
                LogAdd("Недостаточно выносливости!");
                return;
            }
            int damage = Player.equippedweapon.dmg;
            enemy.TakeDamage(damage);
            LogAdd($"{Player.Name} наносит {damage} урона {enemy.Name}");
            if (enemy.HP <= 0)
            {
                LogAdd($"{enemy.Name} повержен!");
                Player.RewardStamina();
                if (TurnCount % 5 == 0)
                {
                    bossDefeatedCount++;
                    LogAdd("!!! БОСС ПОВЕРЖЕН !!!");
                    OnPropertyChanged(nameof(BackgroundImagePath));
                }
                AfterBattleChest = true;
                NextTurn();
                UpdateUI();
                return;
            }
            enemy.AttackPlayer(Player);
            Player.RegenerateStamina();
            Player.ResetTurnStates();
            if (Player.HP <= 0)
            {
                LogAdd("ВЫ ПОГИБЛИ...");
                Pr16.MainWindow.Instance.MainFrame.Navigate(new GameOverPage());
            }
            UpdateUI();
        }

        public void Defend()
        {
            if (!(CurrentEncounter is Enemy enemy)) return;
            if (Player.isFrozen)
            {
                LogAdd($"{Player.Name} заморожен и пропускает ход!");
                Player.isFrozen = false;
                enemy.AttackPlayer(Player);
                Player.RegenerateStamina();
                UpdateUI();
                return;
            }
            Player.isDefending = true;
            LogAdd($"{Player.Name} защищается");
            enemy.AttackPlayer(Player);
            Player.RegenerateStamina();
            Player.ResetTurnStates();
            if (Player.HP <= 0)
            {
                LogAdd("ВЫ ПОГИБЛИ...");
                Pr16.MainWindow.Instance.MainFrame.Navigate(new GameOverPage());
            }
            UpdateUI();
        }

        public void UseItem()
        {
            if (ChestItem == null) return;
            if (ChestItem is Weapon w)
            {
                Player.equippedweapon = w;
                LogAdd($"Вы экипировали {w.name}");
            }
            else if (ChestItem is Armor a)
            {
                Player.equippedarmor = a;
                LogAdd($"Вы надели {a.name}");
            }
            else if (ChestItem is Food)
            {
                Player.HP = Player.MaxHP;
                LogAdd("Вы полностью восстановили здоровье!");
            }
            else if (ChestItem is StaminaPotion s)
            {
                int restored = Math.Min(s.staminaamount, Player.MaxStamina - Player.Stamina);
                Player.Stamina += restored;
                LogAdd($"Вы восстановили {restored} выносливости");
            }
            ChestItem = null;
            OnPropertyChanged(nameof(HpText));
            OnPropertyChanged(nameof(StaminaText));
            NextTurn();
        }

        public void SecondChestAction()
        {
            if (ChestItem == null) return;

            bool isConsumable = ChestItem is Food || ChestItem is StaminaPotion;

            if (isConsumable)
            {
                Player.Inventory.Add(ChestItem);

                if (ChestItem is Food f)
                    LogAdd($"Добавлена в инвентарь: Еда ({f.healamount} HP)");
                else if (ChestItem is StaminaPotion s)
                    LogAdd($"Добавлена в инвентарь: Зелье выносливости (+{s.staminaamount})");

                ChestItem = null;
                OnPropertyChanged(nameof(Player));
            }
            else
            {
                LogAdd("Вы оставили предмет в сундуке.");
                ChestItem = null;
            }

            NextTurn();
        }

        public void UseInventoryItem(object item)
        {
            if (item == null) return;
            if (item is Food f)
            {
                Player.HP = Player.MaxHP;
                LogAdd("Вы полностью восстановили здоровье!");
                Player.Inventory.Remove(item);
            }
            else if (item is StaminaPotion s)
            {
                int restored = Math.Min(s.staminaamount, Player.MaxStamina - Player.Stamina);
                Player.Stamina += restored;
                LogAdd($"Вы восстановили {restored} выносливости");
                Player.Inventory.Remove(item);
            }
            OnPropertyChanged(nameof(HpText));
            OnPropertyChanged(nameof(StaminaText));
            OnPropertyChanged(nameof(Player));
        }
    }
}