using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Enumeration;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Ruminoid.Toolbox.Composition.Roslim;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Formatting;

namespace Ruminoid.Toolbox.Composition.Services
{
    public partial class PluginService : IPluginService
    {
        private readonly ILogger<PluginService> _logger;

        private readonly IRoslimGenerator _roslimGenerator;

        public PluginService(
            IRoslimGenerator roslimGenerator,
            ILogger<PluginService> logger)
        {
            _roslimGenerator = roslimGenerator;
            _logger = logger;

            Initialize();
        }

        #region Containers

        public Collection<(IMeta Meta, Assembly Assembly)> MetaCollection { get; } = new();

        public Collection<(OperationAttribute OperationAttribute, IMeta OperationMeta, Type OperationType)>
            OperationCollection { get; } = new();

        private Dictionary<string, (OperationAttribute OperationAttribute, IMeta OperationMeta, IOperation
            Operation)> OperationCache { get; } = new();

        public Collection<(ConfigSectionAttribute ConfigSectionAttribute, Type ConfigSectionType)>
            ConfigSectionCollection { get; } = new();

        public Collection<(FormatterAttribute FormatterAttribute, Type FormatterType)> FormatterCollection
        {
            get;
        } = new();

        private Dictionary<string, (FormatterAttribute FormatterAttribute, IFormatter Formatter)>
            FormatterCache { get; } = new();

        public (OperationAttribute OperationAttribute, IMeta OperationMeta, IOperation Operation)
            GetOperation(string id)
        {
            bool success = OperationCache.TryGetValue(id,
                out (OperationAttribute OperationAttribute, IMeta OperationMeta, IOperation Operation) cached);
            if (success) return cached;

            (OperationAttribute OperationAttribute, IMeta OperationMeta, Type OperationType) tuple =
                OperationCollection.FirstOrDefault(x => x.OperationAttribute.Id == id);

            // ReSharper disable once InvertIf
            if (tuple == default)
            {
                string err = $"找不到 ID 为 {id} 的操作。可能需要安装相关的插件以解决此问题。";
                _logger.LogError(err);
                throw new PluginCompositionException(err);
            }

            (OperationAttribute OperationAttribute, IMeta OperationMeta, IOperation Operation) created =
                new(tuple.OperationAttribute, tuple.OperationMeta,
                    Activator.CreateInstance(tuple.OperationType) as IOperation);

            OperationCache.TryAdd(tuple.OperationAttribute.Id, created);

            return created;
        }

        public (ConfigSectionAttribute ConfigSectionAttribute, ConfigSectionBase ConfigSection)
            CreateConfigSection(string id)
        {
            (ConfigSectionAttribute ConfigSectionAttribute, Type ConfigSectionType) tuple =
                ConfigSectionCollection.FirstOrDefault(x => x.ConfigSectionAttribute.Id == id);

            // ReSharper disable once InvertIf
            if (tuple == default)
            {
                string err = $"找不到 ID 为 {id} 的配置项。可能需要安装相关的插件以解决此问题。";
                _logger.LogError(err);
                throw new PluginCompositionException(err);
            }

            return (tuple.ConfigSectionAttribute,
                Activator.CreateInstance(tuple.ConfigSectionType) as ConfigSectionBase);
        }

        public (FormatterAttribute FormatterAttribute, IFormatter Formatter) GetFormatter(string target)
        {
            bool success = FormatterCache.TryGetValue(target, out (FormatterAttribute, IFormatter) cached);
            if (success) return cached;

            (FormatterAttribute FormatterAttribute, Type FormatterType) tuple =
                FormatterCollection
                    .FirstOrDefault(x => x.FormatterAttribute.Targets.Split('|')
                        .Any(y => FileSystemName.MatchesSimpleExpression(y, target)));

            // ReSharper disable once InvertIf
            if (tuple == default)
            {
                string err = $"找不到目标为 {target} 的格式器。可能需要安装相关的插件以解决此问题。";
                _logger.LogError(err);
                throw new PluginCompositionException(err);
            }

            (FormatterAttribute FormatterAttribute, IFormatter Formatter) created =
                new(tuple.FormatterAttribute, Activator.CreateInstance(tuple.FormatterType) as IFormatter);

            foreach (string s in tuple.FormatterAttribute.Targets.Split('|'))
                FormatterCache.TryAdd(s, created);

            return created;
        }

        #endregion
    }
}
