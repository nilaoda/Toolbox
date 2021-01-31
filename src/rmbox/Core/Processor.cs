using System;
using System.Composition;
using Microsoft.Extensions.Logging;
using Ruminoid.Toolbox.Helpers.CommandLine;

namespace Ruminoid.Toolbox.Core
{
    [Export]
    public sealed class Processor
    {
        public Processor(
            CommandLineHelper commandLineHelper,
            ProjectParser projectParser,
            ILogger<Processor> logger)
        {
            _logger = logger;

            try
            {
                switch (commandLineHelper.Options)
                {
                    case ProcessOptions:
                        projectParser.Parse();
                        break;
                    default:
                        throw new IndexOutOfRangeException("不支持的命令类型。");
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "处理命令时出现错误。");
            }

            _logger.LogInformation("完成了所有的操作。");
        }
        
        private ILogger<Processor> _logger;
    }
}
