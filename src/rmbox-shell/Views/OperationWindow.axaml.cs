using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Ruminoid.Toolbox.Shell.Models;
using Ruminoid.Toolbox.Shell.ViewModels;

namespace Ruminoid.Toolbox.Shell.Views
{
    public class OperationWindow : Window
    {
        public OperationWindow()
        {
            throw new ArgumentException("No OperationModel provided.");
        }

        public OperationWindow(
            OperationModel operationModel)
        {
            DataContext = new OperationWindowViewModel(operationModel);

            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
