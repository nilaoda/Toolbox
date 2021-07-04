using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json.Linq;
using Ruminoid.Common2.Utils.Extensions;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Shell.Models;

namespace Ruminoid.Toolbox.Shell.ViewModels.Project
{
    public class BatchProjectViewModel : ProjectViewModel
    {
        #region Constructor

        public BatchProjectViewModel(
            OperationModel operationModel,
            Collection<(ConfigSectionAttribute ConfigSectionAttribute, object ConfigSection)> configSections)
        {
            OperationModel = operationModel;

            // WARNING
            // 1. All ConfigSections are cast to JObject when Select().
            // 2. IEnumerable are converted to List (not IList) for read + write.
            ConfigSections = new(configSections
                .Select(x => (x.ConfigSectionAttribute, x.ConfigSection.CloneUsingJson()))
                .ToList());

            // Extract BatchIO

            (ConfigSectionAttribute ConfigSectionAttribute, object ConfigSection) ioTuple = ConfigSections
                .SingleOrDefault(x => x.ConfigSectionAttribute.Id == ConfigSectionBase.BatchIOConfigSectionId);

            if (ioTuple == default)
                throw new InvalidOperationException(
                    "Cannot construct BatchProjectViewModel without BatchIOConfigSection.");

            BatchConfig = (JObject) ioTuple.ConfigSection;

            Source = $"批量 - 共 {BatchConfig["inputs"]!.ToObject<List<string>>()!.Count} 个任务";
        }

        #endregion

        #region Fields

        public readonly OperationModel OperationModel;

        public readonly Collection<(ConfigSectionAttribute ConfigSectionAttribute, object ConfigSection)> ConfigSections;

        public readonly JObject BatchConfig;

        #endregion

        #region ProjectViewModel

        public override string OperationName => OperationModel.Name;

        public override string Source { get; } = "";

        #endregion
    }
}
