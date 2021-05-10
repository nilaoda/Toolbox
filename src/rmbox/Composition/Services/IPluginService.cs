using System;
using System.Collections.ObjectModel;
using System.Reflection;
using JetBrains.Annotations;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Formatting;

namespace Ruminoid.Toolbox.Composition.Services
{
    [PublicAPI]
    public interface IPluginService
    {
        #region Containers

        /// <summary>
        /// 元信息的集合。
        /// </summary>
        public Collection<(IMeta Meta, Assembly Assembly)> MetaCollection { get; }

        /// <summary>
        /// 操作的集合。
        /// </summary>
        public Collection<(OperationAttribute OperationAttribute, IMeta OperationMeta, Type OperationType)>
            OperationCollection
        { get; }

        #endregion

        #region Getters

        /// <summary>
        /// 获取操作。
        /// </summary>
        /// <param name="id">操作的 ID。</param>
        /// <returns>操作的元信息，插件的元信息实例和操作实例。</returns>
        public (OperationAttribute OperationAttribute, IMeta OperationMeta, IOperation Operation)
            GetOperation(string id);

        /// <summary>
        /// 创建配置项。
        /// </summary>
        /// <param name="id">配置项 ID。</param>
        /// <param name="args">配置项初始化数据。</param>
        /// <returns>配置项的元信息和配置项实例。</returns>
        public (ConfigSectionAttribute ConfigSectionAttribute, ConfigSectionBase ConfigSection)
            CreateConfigSection(string id, params object[] args);

        /// <summary>
        /// 获取格式器。
        /// </summary>
        /// <param name="target">要格式的目标进程。</param>
        /// <returns>格式器的元信息和格式器实例。</returns>
        public (FormatterAttribute FormatterAttribute, IFormatter Formatter) GetFormatter(string target);

        #endregion
    }
}
