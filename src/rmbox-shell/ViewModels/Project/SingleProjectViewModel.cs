using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json.Linq;
using Ruminoid.Common2.Utils.Extensions;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Shell.Models;

namespace Ruminoid.Toolbox.Shell.ViewModels.Project
{
    public class SingleProjectViewModel : ProjectViewModel
    {
        #region Constructor

        public SingleProjectViewModel(
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

            (ConfigSectionAttribute ConfigSectionAttribute, object ConfigSection)
                ioConfigSection =
                    ConfigSections
                        .FirstOrDefault(
                            x => x.ConfigSectionAttribute.Id ==
                                 ConfigSectionBase.IOConfigSectionId);

            // ReSharper disable once InvertIf
            if (ioConfigSection != default)
            {
                JObject jObject = JObject.FromObject(ioConfigSection.ConfigSection);

                var source = jObject["input"]?.ToString();
                if (string.IsNullOrWhiteSpace(source)) source = jObject["subtitle"]?.ToString();
                if (string.IsNullOrWhiteSpace(source)) source = "（无外部来源）";
                if (!string.IsNullOrWhiteSpace(source)) Source = source;
            }
        }

        #endregion

        #region Fields

        public readonly OperationModel OperationModel;

        public readonly Collection<(ConfigSectionAttribute ConfigSectionAttribute, object ConfigSection)> ConfigSections;

        #endregion

        #region ProjectViewModel

        public override string OperationName => OperationModel.Name;

        public override string Source { get; } = "";

        #endregion
    }
}
