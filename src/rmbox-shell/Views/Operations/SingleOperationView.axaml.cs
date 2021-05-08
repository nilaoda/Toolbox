using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Ruminoid.Toolbox.Shell.Models;
using Ruminoid.Toolbox.Shell.ViewModels.Operations;

namespace Ruminoid.Toolbox.Shell.Views.Operations
{
    public class SingleOperationView : UserControl
    {
        public SingleOperationView() =>
            throw new ArgumentException("No OperationModel provided.");

        public SingleOperationView(
            OperationModel model)
        {
            DataContext = new SingleOperationViewModel(
                model,
                this);

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
