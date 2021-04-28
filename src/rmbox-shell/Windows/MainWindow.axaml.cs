using System;
using System.ComponentModel;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;
using ReactiveUI;
using Ruminoid.Common2.Metro.MetroControls;
using Ruminoid.Common2.Metro.MetroControls.Dialogs;
using Ruminoid.Toolbox.Shell.Services;
using Splat;

namespace Ruminoid.Toolbox.Shell.Windows
{
    public class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel(this);

            Closing += OnClosing;

            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (CloseConfirmed) return;

            e.Cancel = true;

            Observable.FromAsync(
                    () => MessageBox.ShowAndGetResult(
                        "退出",
                        "确定要退出吗？退出前请确保操作和服务都已停止。",
                        sender as Window),
                    RxApp.MainThreadScheduler)
                .Subscribe(result =>
                {
                    if (result) ForceClose();
                });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    public class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel(
            MainWindow window)
        {
            _window = window;

            Locator.Current
                .GetService<QueueService>()
                .Connect()
                .Subscribe(_ => CurrentTabIndex = (int) CommonTabIndex.QueueView);
        }

        private readonly MainWindow _window;

        #region Tab

        private int _currentTabIndex;

        [UsedImplicitly]
        public int CurrentTabIndex
        {
            get => _currentTabIndex;
            set => this.RaiseAndSetIfChanged(ref _currentTabIndex, value);
        }

        #endregion
    }

    public enum CommonTabIndex
    {
        StartView = 0,
        PluginsView,
        QueueView
    }
}
