using Avalonia;
using Avalonia.Markup.Xaml;
using Ruminoid.Common2.Metro.MetroControls;
using Ruminoid.Toolbox.Shell.ViewModels;

namespace Ruminoid.Toolbox.Shell.Views
{
    public class SplashWindow : MetroWindow
    {
        public SplashWindow()
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

        public SplashWindowViewModel ViewModel => DataContext as SplashWindowViewModel;
    }
}
