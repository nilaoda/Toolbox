using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Ruminoid.Toolbox.Shell.Views
{
    public class PluginManagerView : UserControl
    {
        public PluginManagerView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
