using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Ruminoid.Common2.Metro.MetroControls;
using Ruminoid.Toolbox.Shell.Windows;

namespace Ruminoid.Toolbox.Shell.Services
{
    public sealed class OperationService
    {
        #region Items

        private MainWindow _mainWindow;

        private MainWindow MainWindow =>
            _mainWindow ??= (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
                ?.MainWindow as MainWindow;

        private ObservableCollection<ClosableTabItem> Items => MainWindow.ViewModel.Items;

        private readonly List<(ClosableTabItem TabItem, UserControl OperationView)> OperationList = new();

        #endregion

        #region Commands

        public void AddOperation(
            string tabName,
            UserControl operationView)
        {
            ClosableTabItem tabItem = new()
            {
                Header = tabName,
                Content = operationView
            };

            tabItem.TabClosing += OnTabItemClosing;

            OperationList.Add((tabItem, operationView));
            Items.Add(tabItem);
        }

        private void OnTabItemClosing(object sender, RoutedEventArgs _) =>
            CloseOperation(sender as ClosableTabItem);

        public void CloseOperation(
            UserControl operationView) =>
            CloseOperation(
                OperationList
                    .Find(x => Equals(x.OperationView, operationView)).TabItem);

        public void CloseOperation(
            ClosableTabItem tabItem)
        {
            Items.Remove(tabItem);
            OperationList.Remove(
                OperationList.Find(x => Equals(x.TabItem, tabItem)));

            tabItem.TabClosing -= OnTabItemClosing;
        }

        #endregion
    }
}
