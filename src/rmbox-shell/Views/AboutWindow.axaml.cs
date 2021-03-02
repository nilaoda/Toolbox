using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Ruminoid.Toolbox.Shell.Views
{
    public class AboutWindow : Window
    {
        private AboutWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public static void ShowAbout(Window owner)
        {
            AboutWindow window = new AboutWindow();
            window.ShowDialog(owner);
        }
    }
}
