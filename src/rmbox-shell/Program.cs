using System;
using Avalonia;
using Avalonia.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using ReactiveUI;
using Ruminoid.Toolbox.Composition.Roslim;
using Ruminoid.Toolbox.Composition.Services;
using Ruminoid.Toolbox.Shell.Services;
using Splat;

namespace Ruminoid.Toolbox.Shell
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // Magic - github.com/AvaloniaUI/Avalonia/issues/1934
            RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;

            // Initialize Splat
            InitializeSplat();

            // Build Avalonia app
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();

        public static void InitializeSplat()
        {
            IMutableDependencyResolver resolver = Locator.CurrentMutable;

            resolver.RegisterLazySingleton(
                () => new PluginService(
                    new RoslimGenerator(NullLogger<RoslimGenerator>.Instance),
                    NullLogger<PluginService>.Instance),
                typeof(IPluginService));

            resolver.RegisterLazySingleton(
                () => new QueueService(),
                typeof(QueueService));
        }
    }
}
