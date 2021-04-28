using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Ruminoid.Toolbox.Composition.Roslim;
using Ruminoid.Toolbox.Composition.Services;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Core.Parser;
using Ruminoid.Toolbox.Formatting;
using Ruminoid.Toolbox.Helpers.CommandLine;
using Ruminoid.Toolbox.Utils;

namespace Ruminoid.Toolbox
{
    [UsedImplicitly]
    public class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
                Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "rmbox-shell" + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : "")));
            else
                BuildConsoleApp(args);
        }

        private static void BuildConsoleApp(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

#if DEBUG
            if (args.Contains(CommandLineHelper.DebugAttachString))
            {
                Console.WriteLine("Attach debugger and press ENTER to continue...");
                Console.ReadLine();
            }
#endif

            IHost host = CreateHostBuilder(args).Build();
            _ = host.Services.GetService<Processor>();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                {
                    // Register Services

                    // Composition
                    services.AddSingleton(typeof(IRoslimGenerator), typeof(RoslimGenerator));
                    services.AddSingleton(typeof(IPluginService), typeof(PluginService));

                    // Parsers
                    services.AddSingleton(typeof(SingleProjectParser));
                    services.AddSingleton(typeof(QueueProjectParser));
                    services.AddSingleton(typeof(BatchProjectParser));

                    services.AddSingleton(typeof(ProjectParser));

                    // Formatting
                    services.AddSingleton(typeof(FormattingHelper));

                    // Processor (Entry Point)
                    services.AddSingleton(typeof(Processor));
                })
                .ConfigureLogging(builder => builder
#if DEBUG
                    .AddDebug()
#endif
                    .AddConsole(options => options.FormatterName = RmboxConsoleFormatter.RmboxConsoleFormatterName)
                    .AddConsoleFormatter<RmboxConsoleFormatter, ConsoleFormatterOptions>());
    }
}
