using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using Pr16.Models.Entities;
using Pr16.Models.Factory;
using Pr16.Models.Items;
using Pr16.Services;

namespace Pr16.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        public Player Player { get; set; }
        public object CurrentEncounter { get; set; }
        public object ChestItem { get; set; }

        public int TurnCount { get; private set; }
        public bool AfterBattleChest { get; set; }

        public ObservableCollection<string> Log { get; } = new ObservableCollection<string>();

        private int bossDefeatedCount;
        private string[] backgroundImages;

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

        public Visibility AddToInventoryVisibility
        {
            get { return CanAddToInventory ? Visibility.Visible : Visibility.Collapsed; }
        }

        public bool CanAddToInventory
        {
            get
            {
                if (ChestItem == null) return false;

                if (ChestItem is Weapon) return false; // оружие в инвентарь НЕ кладём

                return ChestItem is Food || ChestItem is StaminaPotion;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public GameViewModel()
        {
            Log.Add("Добро пожаловать в подземелье...");
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
            Log.Add("");
            Log.Add($"──── Ход {TurnCount} ────");

            if (TurnCount % 5 == 0)
            {
                Enemy boss = EnemyFactory.CreateBoss();
                CurrentEncounter = boss;
                Log.Add($"!!! ПОЯВИЛСЯ БОСС: {boss.Name} !!!");

                OnPropertyChanged(nameof(CurrentEncounter));
                OnPropertyChanged(nameof(EncounterImagePath));
                OnPropertyChanged(nameof(BackgroundImagePath));
                OnPropertyChanged(nameof(FightingControlsVisibility));
                OnPropertyChanged(nameof(ChestControlsVisibility));
                OnPropertyChanged(nameof(CanAddToInventory));
                OnPropertyChanged(nameof(AddToInventoryVisibility));
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
            CurrentEncounter = enemy;
            Log.Add($"Появился враг: {enemy.Name} (HP: {enemy.HP})");

            OnPropertyChanged(nameof(CurrentEncounter));
            OnPropertyChanged(nameof(EncounterImagePath));
            OnPropertyChanged(nameof(FightingControlsVisibility));
            OnPropertyChanged(nameof(ChestControlsVisibility));
            OnPropertyChanged(nameof(CanAddToInventory));
            OnPropertyChanged(nameof(AddToInventoryVisibility));
        }

        private void SpawnChest()
        {
            ChestItem = GenerateRandomItem();
            CurrentEncounter = new ChestState { Item = ChestItem };
            Log.Add("Вы нашли сундук!");
            Log.Add($"Внутри: {GetItemName(ChestItem)}");

            OnPropertyChanged(nameof(CurrentEncounter));
            OnPropertyChanged(nameof(EncounterImagePath));
            OnPropertyChanged(nameof(FightingControlsVisibility));
            OnPropertyChanged(nameof(ChestControlsVisibility));
            OnPropertyChanged(nameof(CanAddToInventory));
            OnPropertyChanged(nameof(AddToInventoryVisibility));
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

        public void Attack()
        {
            if (!(CurrentEncounter is Enemy enemy)) return;

            int damage = Player.equippedweapon.dmg;
            enemy.TakeDamage(damage);
            Log.Add($"{Player.Name} наносит {damage} урона {enemy.Name}");

            if (enemy.HP <= 0)
            {
                Log.Add($"{enemy.Name} повержен!");
                Player.RewardStamina();

                if (TurnCount % 5 == 0)
                {
                    bossDefeatedCount++;
                    Log.Add("!!! БОСС ПОВЕРЖЕН! Фон подземелья изменился !!!");
                    OnPropertyChanged(nameof(BackgroundImagePath));
                }

                AfterBattleChest = true;
                NextTurn();
                return;
            }

            // Ответный удар врага
            int enemyDamage = Math.Max(0, enemy.attack - Player.equippedarmor.defense);
            Player.HP -= enemyDamage;
            Log.Add($"{enemy.Name} наносит {enemyDamage} урона {Player.Name}");

            Player.RegenerateStamina();

            if (Player.HP <= 0)
            {
                Log.Add("ВЫ ПОГИБЛИ...");
            }

            OnPropertyChanged(nameof(Player));
            OnPropertyChanged(nameof(CurrentEncounter));
            OnPropertyChanged(nameof(FightingControlsVisibility));
        }

        public void Defend()
        {
            if (!(CurrentEncounter is Enemy enemy)) return;

            Player.isDefending = true;
            Log.Add($"{Player.Name} занимает оборонительную позицию");

            enemy.AttackPlayer(Player);
            Log.Add($"{enemy.Name} наносит удар по обороняющемуся {Player.Name}");

            Player.isDefending = false;
            Player.RegenerateStamina();

            OnPropertyChanged(nameof(Player));
        }

        public void UseItem()
        {
            if (ChestItem == null) return;

            if (ChestItem is Weapon w)
            {
                Player.equippedweapon = w;
                Log.Add($"Вы экипировали {w.name}");
            }
            else if (ChestItem is Armor a)
            {
                Player.equippedarmor = a;
                Log.Add($"Вы надели {a.name}");
            }
            else if (ChestItem is Food f)
            {
                int healed = Math.Min(f.healamount, Player.MaxHP - Player.HP);
                Player.HP += healed;
                Log.Add($"Вы съели еду и восстановили {healed} HP");
            }
            else if (ChestItem is StaminaPotion s)
            {
                int restored = Math.Min(s.staminaamount, Player.MaxStamina - Player.Stamina);
                Player.Stamina += restored;
                Log.Add($"Вы выпили зелье и восстановили {restored} выносливости");
            }

            ChestItem = null;
            OnPropertyChanged(nameof(CanAddToInventory));
            OnPropertyChanged(nameof(AddToInventoryVisibility));
            NextTurn();
        }

        public void SkipItem()
        {
            Log.Add("Вы оставили предмет в сундуке.");
            ChestItem = null;
            OnPropertyChanged(nameof(CanAddToInventory));
            OnPropertyChanged(nameof(AddToInventoryVisibility));
            NextTurn();
        }

        public void AddToInventory()
        {
            if (ChestItem == null) return;

            // Только еда и зелье
            if (!(ChestItem is Food || ChestItem is StaminaPotion)) return;

            Player.Inventory.Add(ChestItem);

            if (ChestItem is Food f)
                Log.Add($"Вы положили в инвентарь еду ({f.healamount} HP)");
            else if (ChestItem is StaminaPotion s)
                Log.Add($"Вы положили в инвентарь зелье выносливости (+{s.staminaamount})");

            ChestItem = null;
            OnPropertyChanged(nameof(CanAddToInventory));
            OnPropertyChanged(nameof(AddToInventoryVisibility));
            OnPropertyChanged(nameof(Player.Inventory)); // обновляем инвентарь в UI
            NextTurn();
        }

        public void UseInventoryItem(object item)
        {
            if (item == null) return;

            if (item is Food f)
            {
                int healed = Math.Min(f.healamount, Player.MaxHP - Player.HP);
                Player.HP += healed;
                Log.Add($"Вы использовали еду из инвентаря и восстановили {healed} HP");
                Player.Inventory.Remove(item);
            }
            else if (item is StaminaPotion s)
            {
                int restored = Math.Min(s.staminaamount, Player.MaxStamina - Player.Stamina);
                Player.Stamina += restored;
                Log.Add($"Вы использовали зелье из инвентаря и восстановили {restored} выносливости");
                Player.Inventory.Remove(item);
            }

            OnPropertyChanged(nameof(Player));
            OnPropertyChanged(nameof(Player.Inventory));
        }
    }
}