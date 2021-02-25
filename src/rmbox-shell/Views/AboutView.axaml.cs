using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Ruminoid.Toolbox.Shell.Views
{
    public class AboutView : UserControl
    {
        public AboutView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
