using System.Windows;
using System.Windows.Controls;
using Pr16.ViewModels;
using Pr16.Models.Items;
using System;

namespace Pr16.Pages
{
    public partial class GamePage : Page
    {
        public GamePage()
        {
            InitializeComponent();
            if (DataContext is GameViewModel vm)
            {
                vm.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(GameViewModel.LogText))
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            ScrollLogToEnd();
                        }));
                    }
                };
            }
        }   

        private void Attack_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is GameViewModel vm) vm.Attack();
        }

        private void Defend_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is GameViewModel vm) vm.Defend();
        }

        private void UseItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is GameViewModel vm) vm.UseItem();
        }

        private void SecondChestAction_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is GameViewModel vm) vm.SecondChestAction();
        }

        private void InventoryItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is object item && DataContext is GameViewModel vm)
            {
                vm.UseInventoryItem(item);
            }
        }
        private void ScrollLogToEnd()
        {
            if (LogScrollViewer != null)
            {
                LogScrollViewer.ScrollToEnd();
            }
        }

    }
}