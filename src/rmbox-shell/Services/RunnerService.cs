using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ReactiveUI;
using Ruminoid.Toolbox.Shell.ViewModels;

namespace Ruminoid.Toolbox.Shell.Services
{
    internal class RunnerService
    {
        #region Constructor

        public RunnerService(
            QueueService queueService)
        {
            _queueService = queueService;

            _queueService
                .WhenAnyValue(x => x.CurrentProject)
                .ObserveOn(Scheduler.Default)
                .Subscribe(TriggerRun);
        }

        #endregion

        #region Data

        private ProjectViewModel _currentProject => _queueService.CurrentProject;

        #endregion

        #region Services

        private readonly QueueService _queueService;

        #endregion

        #region Operations
        
        private void TriggerRun(ProjectViewModel project)
        {
            if (_currentProject is null) return;

            throw new NotImplementedException();
        }

        internal void Kill()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
