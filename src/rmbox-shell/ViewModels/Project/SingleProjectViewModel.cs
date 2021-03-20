using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json.Linq;
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
            ConfigSections = configSections;

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

        public override string Source { get; }

        #endregion
    }
}
