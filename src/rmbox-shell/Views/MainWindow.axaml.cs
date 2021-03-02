using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Ruminoid.Toolbox.Shell.ViewModels;

namespace Ruminoid.Toolbox.Shell.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel(this);

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
