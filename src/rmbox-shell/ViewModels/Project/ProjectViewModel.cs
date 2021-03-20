using System;
using ReactiveUI;

namespace Ruminoid.Toolbox.Shell.ViewModels.Project
{
    public abstract class ProjectViewModel : ReactiveObject
    {
        #region Properties

        public abstract string OperationName { get; }

        public abstract string Source { get; }

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

        private double _progress;
        private bool _isIndeterminate;
        private string _summary = "";
        private string _detail = "";

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
