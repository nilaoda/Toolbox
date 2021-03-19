using System;
using System.Composition;
using Microsoft.Extensions.Logging;
using Ruminoid.Toolbox.Core.Parser;
using Ruminoid.Toolbox.Helpers.CommandLine;

namespace Ruminoid.Toolbox.Core
{
    [Export]
    public sealed class Processor
    {
        public Processor(
            CommandLineHelper commandLineHelper,
            ProjectParser projectParser,
            ProcessRunner processRunner,
            ILogger<Processor> logger)
        {
            _logger = logger;

            try
            {
                switch (commandLineHelper.Options)
                {
                    case ProcessOptions:
                        processRunner.Run(projectParser.Parse());
                        break;
                    default:
                        throw new IndexOutOfRangeException("不支持的命令类型。");
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "处理命令时出现错误。");
                Environment.Exit(1);
            }

            _logger.LogInformation("完成了所有的操作。");
        }
        
        private readonly ILogger<Processor> _logger;
    }
}
