using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Ruminoid.Toolbox.Shell.Views;

namespace Ruminoid.Toolbox.Shell
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                SplashWindow splash = new SplashWindow();
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
}
