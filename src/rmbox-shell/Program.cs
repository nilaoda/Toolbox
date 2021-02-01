using Avalonia;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace Ruminoid.Toolbox.Shell
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Build Host
            IHost host =
                Toolbox.Program
                    .CreateHostBuilder(args, "shell")
                    .ConfigureSplat()
                    .Build();

            // Build Avalonia app
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
    }

    public static class ProgramExtensions
    {
        public static IHostBuilder ConfigureSplat(this IHostBuilder hostBuilder) =>
            hostBuilder
                .ConfigureServices(services =>
                {
                    // Initialize Splat
                    services.UseMicrosoftDependencyResolver();
                    Locator.CurrentMutable.InitializeSplat();
                    Locator.CurrentMutable.InitializeReactiveUI();
                });
    }
}
