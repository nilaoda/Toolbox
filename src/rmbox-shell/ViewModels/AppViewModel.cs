using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using Ruminoid.Toolbox.Shell.Views;
using Ruminoid.Toolbox.Shell.Windows;

namespace Ruminoid.Toolbox.Shell.ViewModels
{
    public class AppViewModel : ReactiveObject
    {
        public void DoShowAboutWindow()
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime
            {
                MainWindow: MainWindow mainWindow
            })
                new AboutWindow().ShowDialog(mainWindow);
        }
    }
}
