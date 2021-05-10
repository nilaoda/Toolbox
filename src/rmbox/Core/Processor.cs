using System;
using Microsoft.Extensions.Logging;
using Ruminoid.Toolbox.Core.Parser;
using Ruminoid.Toolbox.Services.CommandLine;

namespace Ruminoid.Toolbox.Core
{
    public sealed class Processor
    {
        public Processor(
            CommandLineService commandLineService,
            ProjectParser projectParser,
            ProcessRunner processRunner,
            ILogger<Processor> logger)
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                logger.LogCritical("发生了灾难性故障。请联系开发者反馈错误。\n" +
                                   ((e.ExceptionObject as Exception)?.Message ?? string.Empty));

            try
            {
                switch (commandLineService.Options)
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
