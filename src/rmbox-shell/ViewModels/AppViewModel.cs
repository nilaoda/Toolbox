using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using Ruminoid.Toolbox.Shell.Views;

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
                AboutWindow.ShowAbout(mainWindow);
        }
    }
}
