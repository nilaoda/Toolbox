using System;
using System.Composition;
using System.Reflection;
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

            _logger.LogInformation("Ruminoid Toolbox");
            _logger.LogInformation("版本 " + Assembly.GetExecutingAssembly().GetName().Version);
            _logger.LogInformation("启动时使用：" + Environment.CommandLine);

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
