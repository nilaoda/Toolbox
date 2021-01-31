using System;
using System.Collections.Generic;
using System.Composition;
using System.Reflection;
using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.Logging;

namespace Ruminoid.Toolbox.Helpers.CommandLine
{
    [Export]
    public sealed class CommandLineHelper
    {
        public CommandLineHelper(
            ILogger<CommandLineHelper> logger)
        {
            _logger = logger;

            _logger.LogInformation("Ruminoid Toolbox");
            _logger.LogInformation("版本 " + Assembly.GetExecutingAssembly().GetName().Version);
            _logger.LogInformation("启动时使用：" + Environment.CommandLine);

            _result =
                new Parser(settings =>
                    {
                        settings.EnableDashDash = true;
                        settings.HelpWriter = null;
                        settings.AutoHelp = false;
                        settings.AutoVersion = false;
                    })
                    .ParseArguments
                    <
                        ProcessOptions,
                        object // TODO: Replace this
                    >(Environment.GetCommandLineArgs());

            _result
                .WithParsed(options => Options = options)
                .WithNotParsed(DoErrorHandle);
        }
        
        private readonly ParserResult<object> _result;

        public object Options;

        #region Error Handle

        private void DoErrorHandle(IEnumerable<Error> errors)
        {
            HelpText helpText = HelpText.AutoBuild(
                _result,
                help =>
                {
                    help.AdditionalNewLineAfterOption = false;
                    help.Heading = string.Empty;
                    help.Copyright = string.Empty;
                    help.AutoHelp = false;
                    help.AutoVersion = false;

                    return help;
                });

            Console.WriteLine(helpText);

            Environment.Exit(1);
        }

        #endregion

        private ILogger<CommandLineHelper> _logger;
    }
}
