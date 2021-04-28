using System;
using Microsoft.Extensions.Logging;
using Ruminoid.Toolbox.Core.Parser;
using Ruminoid.Toolbox.Helpers.CommandLine;

namespace Ruminoid.Toolbox.Core
{
    public sealed class Processor
    {
        public Processor(
            CommandLineHelper commandLineHelper,
            ProjectParser projectParser,
            ProcessRunner processRunner,
            ILogger<Processor> logger)
        {
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
                logger.LogCritical(e, "处理命令时出现错误。");
                Environment.Exit(1);
            }

            logger.LogInformation("完成了所有的操作。");
        }
    }
}
