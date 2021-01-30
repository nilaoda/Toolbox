using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
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
            _logger.LogDebug("Parsing projectPath:");
            _logger.LogDebug(path);

            try
            {
                _logger.LogInformation("开始解析 JSON 项目。");

                string projectRaw = File.ReadAllText(Path.GetFullPath(path));
                JObject project = JObject.Parse(projectRaw);

                _logger.LogDebug("Validating project.");
                AssertValidVersion(project);
                AssertValidType(project, "project");

                _logger.LogDebug("Collecting sections.");
                JToken sections = project["sections"];

                if (sections is null)
                {
                    ArgumentNullException e = new ArgumentNullException(nameof(sections));
                    _logger.LogCritical(e, "Sections is null.");
                    throw e;
                }

                _logger.LogDebug($"Collected {sections.Count()} section(s).");

                IEnumerable<KeyValuePair<string, JToken>> sectionData;

                try
                {
                    sectionData =
                        sections.Select(
                            section => new KeyValuePair<string, JToken>(section["type"].ToObject<string>(),
                                section["data"]));

                    if (sectionData is null)
                        throw new ArgumentNullException(nameof(sectionData));
                }
                catch (Exception e)
                {
                    const string err = "解析配置项时发生了错误。";
                    _logger.LogCritical(e, err);
                    throw new ProjectParseException(err, e);
                }

                _logger.LogInformation($"解析了 {sectionData.Count()} 个配置项。");
                _logger.LogInformation("开始生成运行。");

                // TODO: Get Operation
                // TODO: Get List<KayValuePair<string, string>>

                _logger.LogInformation("开始运行。");

                // TODO: Run Process
            }
            catch (Exception e)
            {
                const string err = "解析项目文件时发生了错误。";
                _logger.LogCritical(e, err);
                throw new ProjectParseException(err, e);
            }
        }

        #region Parse Utils

        public const int ValidProjectVersion = 1;

        public void AssertValidType(JObject project, string type)
        {
            string projectType = project["type"].ToObject<string>();

            if (projectType == type) return;

            string err = $"项目文件的类型是 {projectType}，但应为 {type}。";
            _logger.LogCritical(err);
            throw new Exception(err);
        }

        private void AssertValidVersion(JObject project)
        {
            // ReSharper disable once PossibleNullReferenceException
            if ((_commandLineHelper.Options as ProcessOptions).SkipVersionCheck) return;

            int version = project["version"].ToObject<int>();

            if (version == ValidProjectVersion) return;

            string err = $"项目文件的版本（{version}）与程序支持的版本（{ValidProjectVersion}）不匹配。使用 --skip-version-check 跳过版本检查。";
            _logger.LogCritical(err);
            throw new Exception(err);
        }

        #endregion

        private readonly CommandLineHelper _commandLineHelper;
        private readonly ILogger<ProjectParser> _logger;
    }
}
