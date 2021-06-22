using System;
using System.Collections.ObjectModel;
using System.Linq;
using Ruminoid.Common2.Utils.Extensions;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Shell.Models;
using Ruminoid.Toolbox.Shell.Utils.ConfigSections;

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
            ConfigSections = new(configSections
                .Select(x => (x.ConfigSectionAttribute, x.ConfigSection.CloneUsingJson()))
                .ToArray());

            // Extract BatchIO

            (ConfigSectionAttribute ConfigSectionAttribute, object ConfigSection) ioTuple = ConfigSections
                .SingleOrDefault(x => x.ConfigSectionAttribute.Id == ConfigSectionBase.BatchIOConfigSectionId);

            if (ioTuple == default)
                throw new InvalidOperationException(
                    "Cannot construct BatchProjectViewModel without BatchIOConfigSection.");

            ConfigSections.Remove(ioTuple);

            BatchConfig =
                new(ioTuple.ConfigSection as BatchIOConfigSectionViewModel);

            Source = $"批量 - 共 {BatchConfig.InputList.Count} 个任务";
        }

        #endregion

        #region Fields

        public readonly OperationModel OperationModel;

        public readonly Collection<(ConfigSectionAttribute ConfigSectionAttribute, object ConfigSection)> ConfigSections;

        public readonly BatchIOConfigSectionViewModel BatchConfig;

        #endregion

        #region ProjectViewModel

        public override string OperationName => OperationModel.Name;

        public override string Source { get; } = "";

        #endregion
    }
}
