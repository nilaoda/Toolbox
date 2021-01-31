using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Composition;
using Ruminoid.Toolbox.Helpers.CommandLine;

namespace Ruminoid.Toolbox.Core
{
    [Export]
    public class ProjectParser
    {
        public ProjectParser(
            CommandLineHelper commandLineHelper,
            PluginHelper pluginHelper,
            ProcessRunner processRunner,
            ILogger<ProjectParser> logger)
        {
            _commandLineHelper = commandLineHelper;
            _pluginHelper = pluginHelper;
            _processRunner = processRunner;
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

            List<Tuple<string, string>> commands;

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

                Dictionary<string, JToken> sectionData = new Dictionary<string, JToken>();

                try
                {
                    foreach (Tuple<string, JToken> tuple in sections.Select(
                        section => new Tuple<string, JToken>(
                            section["type"].ToObject<string>(),
                            section["data"])))
                    {
                        sectionData.Add(tuple.Item1, tuple.Item2);
                    }

                    if (sectionData.Count == 0)
                    {
                        const string err = "项目中没有有效的配置项。";
                        _logger.LogError(err);
                        throw new IndexOutOfRangeException(err);
                    }
                }
                catch (Exception e)
                {
                    const string err = "解析配置项时发生了错误。";
                    _logger.LogCritical(e, err);
                    throw new ProjectParseException(err, e);
                }

                _logger.LogInformation($"解析了 {sectionData.Count} 个配置项。");
                _logger.LogInformation("开始生成运行。");
                
                string operationId;

                try
                {
                    // ReSharper disable once PossibleNullReferenceException
                    operationId = project["operation"].ToObject<string>();

                    if (string.IsNullOrWhiteSpace(operationId))
                        throw new ArgumentNullException(nameof(operationId));
                }
                catch (Exception e)
                {
                    const string err = "解析操作时发生了错误。";
                    _logger.LogCritical(e, err);
                    throw new ProjectParseException(err, e);
                }

                Tuple<OperationAttribute, IOperation> operation;

                try
                {
                    operation = _pluginHelper.GetOperation(operationId);

                    if (operation is null)
                        throw new ArgumentNullException(nameof(operation));
                }
                catch (Exception e)
                {
                    string err = $"获取操作 {operationId} 时发生了错误。";
                    _logger.LogCritical(e, err);
                    throw new ProjectParseException(err, e);
                }

                _logger.LogInformation($"使用 {operation.Item1.Name} 进行操作。");
                _logger.LogDebug($"Checking RequiredConfigSections for operation {operation.Item1.Name}");

                try
                {
                    commands = operation.Item2.Generate(sectionData);

                    if (commands is null)
                        throw new ArgumentNullException(nameof(commands));

                    if (commands.Count == 0)
                    {
                        _logger.LogWarning("操作没有生成任何有效的命令，因此程序不需要做什么。");

                        // WARNING
                        // Method returned without cleaning up.
                        // Check if other resources needs dispose.
                        return;
                    }
                }
                catch (Exception e)
                {
                    string err = $"操作 {operationId} 获取指令时发生了错误。";
                    _logger.LogCritical(e, err);
                    throw new ProjectParseException(err, e);
                }

                _logger.LogInformation($"生成了 {commands.Count} 个指令。");
            }
            catch (Exception e)
            {
                const string err = "解析项目文件时发生了错误。";
                _logger.LogCritical(e, err);
                throw new ProjectParseException(err, e);
            }
            
            _logger.LogInformation("开始运行。");

            _processRunner.Run(commands);
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
        private readonly PluginHelper _pluginHelper;
        private readonly ProcessRunner _processRunner;
        private readonly ILogger<ProjectParser> _logger;
    }
}
