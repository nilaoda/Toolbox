using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Ruminoid.Toolbox.Shell.ViewModels;

namespace Ruminoid.Toolbox.Shell.Views
{
    public class AboutWindow : Window
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public AboutWindow()
        {
            DataContext = new AboutWindowViewModel(this);

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
