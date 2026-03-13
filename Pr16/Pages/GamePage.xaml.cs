using System.Windows;
using System.Windows.Controls;
using Pr16.ViewModels;
using System.Collections.Specialized;
using Pr16.Models.Items;
using System.Windows.Media;
using System;

namespace Pr16.Pages
{
    public partial class GamePage : Page
    {
        public GamePage()
        {
            InitializeComponent();
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

        private void SkipItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is GameViewModel vm) vm.SkipItem();
        }

        private void InventoryItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is object item && DataContext is GameViewModel vm)
            {
                vm.UseInventoryItem(item);
            }
        }

        private void LogListBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is GameViewModel vm)
            {
                // Подписка на изменение коллекции
                vm.Log.CollectionChanged += Log_CollectionChanged;
                // Скролл к концу после загрузки
                Dispatcher.BeginInvoke(new Action(ScrollToEnd));
            }
        }

        private void Log_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(ScrollToEnd));
        }

        private void ScrollToEnd()
        {
            if (LogListBox == null || LogListBox.Items.Count == 0) return;

            // Проверяем, что мы уже в конце — если да, скроллим к последнему
            var scrollViewer = FindVisualChild<ScrollViewer>(LogListBox);
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToEnd();
            }
            else
            {
                LogListBox.ScrollIntoView(LogListBox.Items[LogListBox.Items.Count - 1]);
            }
        }

        private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;

                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }
    }
}