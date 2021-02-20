using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using Ruminoid.Toolbox.Shell.ViewModels;

namespace Ruminoid.Toolbox.Shell.Services
{
    public class QueueService : ReactiveObject
    {
        #region Constructor

        public QueueService()
        {
            Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(ItemsChanged);

            this
                .WhenAnyValue(x => x.CurrentProject)
                .ObserveOn(Scheduler.Default)
                .Subscribe(CurrentProjectChanged)
        }

        #endregion

        #region Data

        private readonly SourceCache<ProjectViewModel, Guid> _items = new(x => x.Id);

        public IObservable<IChangeSet<ProjectViewModel, Guid>> Connect() => _items.Connect();

        #endregion

        #region Current

        private ProjectViewModel _currentProject = null;

        public ProjectViewModel CurrentProject
        {
            get => _currentProject;
            set
            {
                if (value.Status != ProjectStatus.Waiting)
                    throw new InvalidOperationException();

                value.Status = ProjectStatus.Running;
                value.IsIndeterminate = true;
                value.Summary = "准备启动运行";

                this.RaiseAndSetIfChanged(ref _currentProject, value);
            }
        }

        #endregion

        #region Dispatcher

        private void ItemsChanged(IChangeSet<ProjectViewModel, Guid> obj)
        {
            if (!_queueRunning || _currentProject is not null)
                return;
        }

        private void CurrentProjectChanged(ProjectViewModel obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Status

        private bool _queueRunning = true;

        #endregion

        #region Queue Operations

        public void Start() => _queueRunning = true;

        public void Stop() => _queueRunning = false;

        public void Kill()
        {

        }

        #endregion

        #region Operations

        public void AddProject(
            ProjectViewModel project) =>
            _items.AddOrUpdate(project);

        #endregion
    }
}
