using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.Logging;

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
                    .Build()
                    .ConfigureContainerForSplat();

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
                })
                .ConfigureLogging(builder => builder.AddSplat());

        public static IHost ConfigureContainerForSplat(this IHost host)
        {
            host.Services.UseMicrosoftDependencyResolver();
            
            Locator.CurrentMutable
                .UseMicrosoftExtensionsLoggingWithWrappingFullLogger(
                    host.Services.GetService<ILoggerFactory>());

            return host;
        }
    }
}
