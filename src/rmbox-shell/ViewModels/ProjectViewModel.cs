using System;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Shell.Models;

namespace Ruminoid.Toolbox.Shell.ViewModels
{
    public class ProjectViewModel : ReactiveObject
    {
        #region Constructor

        public ProjectViewModel(
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
                                 "Ruminoid.Toolbox.Plugins.Common.ConfigSections.IOConfigSection");

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

        #region Properties

        public string OperationName => OperationModel.Name;

        public string Source { get; } = "";

        public Guid Id { get; } = Guid.NewGuid();

        #endregion

        #region Status Data

        private ProjectStatus _status = ProjectStatus.Queued;

        public ProjectStatus Status
        {
            get => _status;
            set => this.RaiseAndSetIfChanged(ref _status, value);
        }

        #endregion

        #region FormattedEvent Data

        private string _target = "";
        private double _progress;
        private bool _isIndeterminate;
        private string _summary = "";
        private string _detail = "";

        public string Target
        {
            get => _target;
            set => this.RaiseAndSetIfChanged(ref _target, value);
        }

        public double Progress
        {
            get => _progress;
            set => this.RaiseAndSetIfChanged(ref _progress, value);
        }

        public bool IsIndeterminate
        {
            get => _isIndeterminate;
            set => this.RaiseAndSetIfChanged(ref _isIndeterminate, value);
        }

        public string Summary
        {
            get => _summary;
            set => this.RaiseAndSetIfChanged(ref _summary, value);
        }

        public string Detail
        {
            get => _detail;
            set => this.RaiseAndSetIfChanged(ref _detail, value);
        }

        #endregion
    }

    public enum ProjectStatus
    {
        Queued = 0,
        Running = 1,
        Completed = 2,
        Error = 3
    }
}
