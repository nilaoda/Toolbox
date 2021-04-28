using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Composition.Services;

namespace Ruminoid.Toolbox.Core.Parser
{
    public class SingleProjectParser : IParser
    {
        public SingleProjectParser(
            IPluginService pluginService,
            ILogger<SingleProjectParser> logger)
        {
            _pluginService = pluginService;
            _logger = logger;
        }

        public List<TaskCommand> Parse(JToken project)
        {
            try
            {
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
                    foreach ((string ConfigSectionId, JToken ConfigSection) tuple in sections.Select(
                        section => (section["type"].ToObject<string>(), section["data"])))
                    {
                        sectionData.Add(tuple.ConfigSectionId, tuple.ConfigSection);
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

                (OperationAttribute OperationAttribute, IMeta OperationMeta, IOperation Operation) operation;

                try
                {
                    operation = _pluginService.GetOperation(operationId);

                    if (operation == default)
                        throw new ArgumentNullException(nameof(operation));
                }
                catch (Exception e)
                {
                    string err = $"获取操作 {operationId} 时发生了错误。";
                    _logger.LogCritical(e, err);
                    throw new ProjectParseException(err, e);
                }

                _logger.LogInformation($"使用 {operation.OperationAttribute.Name} 进行操作。");
                _logger.LogDebug($"Checking RequiredConfigSections for operation {operation.OperationAttribute.Name}");

                List<TaskCommand> commands;

                try
                {
                    commands = operation.Operation.Generate(sectionData);

                    if (commands is null)
                        throw new ArgumentNullException(nameof(commands));

                    if (commands.Count == 0)
                    {
                        _logger.LogWarning("操作没有生成任何有效的命令，因此程序不需要做什么。");

                        // WARNING
                        // Method returned without cleaning up.
                        // Check if other resources needs dispose.
                        return new List<TaskCommand>();
                    }
                }
                catch (Exception e)
                {
                    string err = $"操作 {operationId} 获取指令时发生了错误。";
                    _logger.LogCritical(e, err);
                    throw new ProjectParseException(err, e);
                }

                _logger.LogInformation($"生成了 {commands.Count} 个指令。");

                return commands;
            }
            catch (Exception e)
            {
                const string err = "解析项目文件时发生了错误。";
                _logger.LogCritical(e, err);
                throw new ProjectParseException(err, e);
            }
        }
        
        private readonly IPluginService _pluginService;
        private readonly ILogger<SingleProjectParser> _logger;
    }
}
