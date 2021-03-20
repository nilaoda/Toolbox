using System;
using System.ComponentModel;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using Ruminoid.Toolbox.Shell.Utils.Dialogs;
using Ruminoid.Toolbox.Shell.Utils.Windows;
using Ruminoid.Toolbox.Shell.ViewModels;

namespace Ruminoid.Toolbox.Shell.Views
{
    public class MainWindow : RmboxWindowBase
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
}
