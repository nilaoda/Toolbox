using System.Linq;
using JetBrains.Annotations;
using ReactiveUI;
using Ruminoid.Toolbox.Shell.ViewModels.Project;

namespace Ruminoid.Toolbox.Shell.Views
{
    public partial class QueueViewModel
    {
        private readonly ObservableAsPropertyHelper<string> _statusSummary;

        [UsedImplicitly]
        public string StatusSummary => _statusSummary.Value;

        private readonly ObservableAsPropertyHelper<string> _statusDetail;

        [UsedImplicitly]
        public string StatusDetail => _statusDetail.Value;

        private QueueStatus _currentStatus = QueueStatus.Ready;

        private QueueStatus CurrentStatus
        {
            get => _currentStatus;
            set => this.RaiseAndSetIfChanged(ref _currentStatus, value);
        }

        private void TriggerStatusUpdate()
        {
            if (_queueService.CurrentProject is not null &&
                _queueService.CurrentProject.Status == ProjectStatus.Error)
            {
                CurrentStatus = QueueStatus.Error;
                return;
            }

            CurrentStatus = (_queueService.CurrentProject is not null) switch
            {
                true => _queueService.QueueRunning switch {
                    true => QueueStatus.Running,
                    false => QueueStatus.RunningWithoutQueue
                },
                false => _queueService.QueueRunning switch
                {
                    true => QueueStatus.Ready,
                    false => QueueStatus.Free
                }
            };
        }
    }

    internal enum QueueStatus
    {
        Ready = 0,
        Free = 1,
        Running = 2,
        RunningWithoutQueue = 3,
        Error = 4
    }
}
