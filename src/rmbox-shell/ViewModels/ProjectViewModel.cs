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
            Collection<Tuple<ConfigSectionAttribute, object>> configSections)
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

        public Guid Id { get; } = new();

        #endregion

        #region Status Data

        private ProjectStatus _status = ProjectStatus.Waiting;

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
        Waiting = 0,
        Running = 1,
        Completed = 2
    }
}
