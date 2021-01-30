using System.Composition;
using System.IO;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Helpers.CommandLine;

namespace Ruminoid.Toolbox.Core
{
    [Export]
    public class ProjectParser
    {
        public ProjectParser(
            CommandLineHelper commandLineHelper,
            ILogger<ProjectParser> logger)
        {
            _commandLineHelper = commandLineHelper;
            _logger = logger;
        }

        /// <summary>
        /// 解析 JSON 项目文件，并执行操作。
        /// </summary>
        public void Parse()
        {
            _logger.LogDebug("Parsing using projectPath from ProcessOptions.");

            // ReSharper disable once PossibleNullReferenceException
            Parse((_commandLineHelper.Options as ProcessOptions).ProjectPath);
        }

        /// <summary>
        /// 解析 JSON 项目文件，并执行操作。
        /// </summary>
        /// <param name="path">JSON 项目文件的路径。</param>
        public void Parse(string path)
        {
            throw new NotImplementedException();
        }

        private readonly CommandLineHelper _commandLineHelper;
        private readonly ILogger<ProjectParser> _logger;
    }
}
