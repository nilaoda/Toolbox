using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Ruminoid.Toolbox.Shell.Views
{
    public class OperationsView : UserControl
    {
        public OperationsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
