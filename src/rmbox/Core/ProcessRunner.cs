using System;
using System.Collections.Generic;
using System.Composition;
using Microsoft.Extensions.Logging;
using Ruminoid.Toolbox.Composition;
using Ruminoid.Toolbox.Helpers.CommandLine;

namespace Ruminoid.Toolbox.Core
{
    [Export]
    public class ProcessRunner
    {
        public ProcessRunner(
            CommandLineHelper commandLineHelper,
            PluginHelper pluginHelper,
            Logger<ProcessRunner> logger)
        {
            _commandLineHelper = commandLineHelper;
            _pluginHelper = pluginHelper;
            _logger = logger;
        }

        /// <summary>
        /// 执行进程。
        /// </summary>
        /// <param name="target">进程目标。</param>
        /// <param name="args">进程参数。</param>
        public void Run(string target, string args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 执行进程。
        /// </summary>
        /// <param name="command">指令。</param>
        public void Run(Tuple<string, string> command) =>
            Run(command.Item1, command.Item2);

        /// <summary>
        /// 执行进程。
        /// </summary>
        /// <param name="commands">指令列表。</param>
        public void Run(List<Tuple<string, string>> commands)
        {

        }

        private readonly CommandLineHelper _commandLineHelper;
        private readonly PluginHelper _pluginHelper;
        private readonly ILogger<ProcessRunner> _logger;
    }
}
