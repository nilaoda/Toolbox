using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Ruminoid.Toolbox.Composition;
using Splat;

namespace Ruminoid.Toolbox.Shell.Views
{
    public class OperationsView : UserControl
    {
        public OperationsView()
        {
            PluginHelper = Locator.Current.GetService<PluginHelper>();

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public PluginHelper PluginHelper { get; }
    }
}
