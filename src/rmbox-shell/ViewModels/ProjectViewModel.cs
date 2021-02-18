using ReactiveUI;
using Ruminoid.Toolbox.Shell.Models;

namespace Ruminoid.Toolbox.Shell.ViewModels
{
    public class ProjectViewModel : ReactiveObject
    {
        #region Constructor

        public ProjectViewModel(
            ProjectModel projectModel)
        {
            ProjectModel = projectModel;
        }

        #endregion

        #region ProjectModel Data

        public ProjectModel ProjectModel { get; }

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
