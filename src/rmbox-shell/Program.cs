using Avalonia;
using Microsoft.Extensions.Logging.Abstractions;
using Ruminoid.Toolbox.Composition;
using Splat;

namespace Ruminoid.Toolbox.Shell
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
                () => new PluginHelper(
                    NullLogger<PluginHelper>.Instance),
                typeof(PluginHelper));
        }
    }
}
