using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Services.CommandLine;

namespace Ruminoid.Toolbox.Core.Parser
{
    public class ProjectParser : IParser
    {
        public ProjectParser(
            SingleProjectParser singleProjectParser,
            QueueProjectParser queueProjectParser,
            BatchProjectParser batchProjectParser,
            CommandLineService commandLineService,
            ILogger<ProjectParser> logger)
        {
            _singleProjectParser = singleProjectParser;
            _queueProjectParser = queueProjectParser;
            _batchProjectParser = batchProjectParser;

            _commandLineService = commandLineService;
            _logger = logger;
        }

        /// <summary>
        /// 解析 JSON 项目文件。
        /// </summary>
        public List<TaskCommand> Parse()
        {
            _logger.LogDebug("Parsing using projectPath from ProcessOptions.");

            // ReSharper disable once PossibleNullReferenceException
            return Parse((_commandLineService.Options as ProcessOptions).ProjectPath);
        }

        /// <summary>
        /// 解析 JSON 项目文件。
        /// </summary>
        /// <param name="path">JSON 项目文件的路径。</param>
        private List<TaskCommand> Parse(string path)
        {
            _logger.LogDebug("Parsing projectPath:");
            _logger.LogDebug(path);

            JObject project;

            try
            {
                _logger.LogInformation("开始解析 JSON 项目。");

                string projectRaw = File.ReadAllText(Path.GetFullPath(path));
                project = JObject.Parse(projectRaw);

                if (project is null)
                    throw new ArgumentNullException(nameof(project));
            }
            catch (Exception e)
            {
                const string err = "加载项目文件时发生了错误。";
                _logger.LogCritical(e, err);
                throw new ProjectParseException(err, e);
            }

            return Parse(project);
        }
        
        public List<TaskCommand> Parse(JToken project)
        {
            try
            {
                _logger.LogDebug("Validating project.");

                // ReSharper disable once PossibleNullReferenceException
                if (!(_commandLineService.Options as ProcessOptions).SkipVersionCheck)
                    AssertValidVersion(project);

                string projectType = project["type"].ToObject<string>();

                return projectType switch
                {
                    "project" => _singleProjectParser.Parse(project),
                    "queue" => _queueProjectParser.Parse(project),
                    "batch" => _batchProjectParser.Parse(project),
                    _ => throw new ProjectParseException($"不能识别的项目文件的类型 {projectType}。升级Ruminoid Toolbox到较新版本以解决此问题。")
                };
            }
            catch (Exception e)
            {
                const string err = "解析项目文件时发生了错误。";
                _logger.LogCritical(e, err);
                throw new ProjectParseException(err, e);
            }
        }

        #region Parse Utils

        [PublicAPI]
        public const int ValidProjectVersion = 1;

        [Obsolete]
        public static void AssertValidType(JObject project, string type)
        {
            string projectType = project["type"].ToObject<string>();

            if (projectType == type) return;

            string err = $"项目文件的类型是 {projectType}，但应为 {type}。";
            throw new ProjectParseException(err);
        }

        private static void AssertValidVersion(JToken project)
        {
            int version = project["version"].ToObject<int>();

            if (version == ValidProjectVersion) return;

            string err = $"项目文件的版本（{version}）与程序支持的版本（{ValidProjectVersion}）不匹配。使用 --skip-version-check 跳过版本检查。";
            throw new ProjectParseException(err);
        }

        #endregion

        #region Parsers

        private readonly SingleProjectParser _singleProjectParser;
        private readonly QueueProjectParser _queueProjectParser;
        private readonly BatchProjectParser _batchProjectParser;

        #endregion

        private readonly CommandLineService _commandLineService;
        private readonly ILogger<ProjectParser> _logger;
    }
}
