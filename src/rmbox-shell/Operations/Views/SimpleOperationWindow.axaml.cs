using System;
using System.ComponentModel;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using Ruminoid.Toolbox.Shell.Models;
using Ruminoid.Toolbox.Shell.Operations.ViewModels;
using Ruminoid.Toolbox.Shell.Utils.Dialogs;
using Ruminoid.Toolbox.Shell.Utils.Windows;

namespace Ruminoid.Toolbox.Shell.Operations.Views
{
    public class SimpleOperationWindow : RmboxWindowBase
    {
        public SimpleOperationWindow()
        {
            throw new ArgumentException("No OperationModel provided.");
        }

        public SimpleOperationWindow(
            OperationModel operationModel)
        {
            DataContext = new SimpleOperationWindowViewModel(
                operationModel,
                this);

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
                        "关闭",
                        "确定要关闭操作吗？未导出的配置将不会保留。",
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
