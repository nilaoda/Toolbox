using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Ruminoid.Toolbox.Shell.Models;
using Ruminoid.Toolbox.Shell.ViewModels.Operations;

namespace Ruminoid.Toolbox.Shell.Views.Operations
{
    public class BatchOperationView : UserControl
    {
        public BatchOperationView() =>
            throw new ArgumentException("No OperationModel provided.");

        public BatchOperationView(
            OperationModel model)
        {
            DataContext = new BatchOperationViewModel(
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
