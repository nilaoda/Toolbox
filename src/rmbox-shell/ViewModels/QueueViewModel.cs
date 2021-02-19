using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using Ruminoid.Toolbox.Shell.Services;
using Splat;

namespace Ruminoid.Toolbox.Shell.ViewModels
{
    public class QueueViewModel : ReactiveObject
    {
        #region Constructor

        public QueueViewModel()
        {
            _queueService = Locator.Current
                .GetService<QueueService>();

            _queueService
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _items)
                .Subscribe();
        }

        #endregion

        #region Data
        
        private readonly ReadOnlyObservableCollection<ProjectViewModel> _items;

        public ReadOnlyObservableCollection<ProjectViewModel> Items => _items;

        #endregion

        #region Services

        private readonly QueueService _queueService;

        #endregion
    }
}
