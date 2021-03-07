using System;
using System.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Helpers.CommandLine;
using Ruminoid.Toolbox.Utils;

namespace Ruminoid.Toolbox
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "rmbox-shell" + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : "")));
            else
                BuildConsoleApp(args);
        }
        
        public static void BuildConsoleApp(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            if (args.Contains(CommandLineHelper.DebugAttachString))
            {
                Console.WriteLine("Attach debugger and press ENTER to continue...");
                Console.ReadLine();
            }
            
            IHost host = CreateHostBuilder(args).Build();
            _ = host.Services.GetService<Processor>();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
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
                    .AddConsole(options => options.FormatterName = RmboxConsoleFormatter.RmboxConsoleFormatterName)
                    .AddConsoleFormatter<RmboxConsoleFormatter, ConsoleFormatterOptions>());
    }
}
