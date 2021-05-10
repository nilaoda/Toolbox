using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;
using ReactiveUI;
using Ruminoid.Toolbox.Shell.Windows;

namespace Ruminoid.Toolbox.Shell
{
    public class App : Application
    {
        public App()
        {
            DataContext = new AppViewModel();
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                SplashWindow splash = new();
                splash.Show();

                splash.ViewModel.Initialize
                    .Subscribe(
                        _ => { },
                        _ => { },
                        () =>
                        {
                            desktop.MainWindow.Show();
                            splash.Close();
                        });
            }

            base.OnFrameworkInitializationCompleted();
        }
    }

    public class AppViewModel : ReactiveObject
    {
        [UsedImplicitly]
        public void DoShowAboutWindow()
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime
            {
                MainWindow: MainWindow mainWindow
            })
                mainWindow.ViewModel.CurrentTabIndex = (int) CommonTabIndex.AboutView;
        }
    }
}
