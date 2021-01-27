using System;
using System.Composition;
using System.Linq;
using System.Reflection;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Ruminoid.Toolbox.Shell;

namespace Ruminoid.Toolbox
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            else
                BuildConsoleApp(args);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();

        public static void BuildConsoleApp(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();
            host.Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // Register Services
                    foreach (Type type in AppDomain.CurrentDomain.GetAssemblies()
                        .Concat(Assembly.GetExecutingAssembly().GetReferencedAssemblies()
                            .Select(Assembly.Load))
                        .SelectMany(x =>
                            x.GetTypes()
                                .Where(type =>
                                    Attribute.GetCustomAttribute(type, typeof(ExportAttribute)) is not null)))
                        services.AddSingleton(type);
                })
                .ConfigureLogging(builder => builder
                    .AddDebug()
                    .AddSimpleConsole(options =>
                    {
                        options.ColorBehavior = LoggerColorBehavior.Enabled;
                        options.SingleLine = true;
                        options.IncludeScopes = true;
                    })
                    .AddSystemdConsole());
    }
}
