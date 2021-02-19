using System;
using DynamicData;
using Ruminoid.Toolbox.Shell.ViewModels;

namespace Ruminoid.Toolbox.Shell.Services
{
    public class QueueService
    {
        #region Data

        private readonly SourceCache<ProjectViewModel, Guid> _items = new(x => x.Id);

        public IObservable<IChangeSet<ProjectViewModel, Guid>> Connect() => _items.Connect();

        #endregion

        #region Status

        private bool _queueRunning = true;

        #endregion

        #region Queue Operations

        public void Start()
        {
            if (_queueRunning) return;
        }

        public void Stop()
        {
            if (!_queueRunning) return;
        }

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
