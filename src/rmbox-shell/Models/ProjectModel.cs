using System;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;

namespace Ruminoid.Toolbox.Shell.Models
{
    public class ProjectModel
    {
        #region Constructor

        public ProjectModel(OperationModel operationModel, Collection<Tuple<ConfigSectionAttribute, object>> configSections)
        {
            _operationModel = operationModel;
            _configSections = configSections;

            Tuple<ConfigSectionAttribute, object>
                ioConfigSection =
                    _configSections
                        .FirstOrDefault(
                            x => x.Item1.Id ==
                                 "Ruminoid.Toolbox.Plugins.Common.ConfigSections.IOConfigSection");

            // ReSharper disable once InvertIf
            if (ioConfigSection is not null)
            {
                JObject jObject = JObject.FromObject(ioConfigSection.Item2);

                var source = jObject["video"]?.ToString();
                if (string.IsNullOrWhiteSpace(source)) source = jObject["subtitle"]?.ToString();
                if (!string.IsNullOrWhiteSpace(source)) Source = source;
            }
        }

        #endregion

        #region Fields

        private readonly OperationModel _operationModel;

        private readonly Collection<Tuple<ConfigSectionAttribute, object>> _configSections;

        #endregion

        #region Properties

        public string OperationName => _operationModel.Name;

        public string Source { get; } = "";

        #endregion
    }
}
