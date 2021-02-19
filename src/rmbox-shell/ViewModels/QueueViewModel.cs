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

        private ProjectViewModel _selectedItem;

        public ProjectViewModel SelectedItem
        {
            get => _selectedItem;
            set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }

        private bool _queueRunning = true;

        public bool QueueRunning
        {
            get => _queueRunning;
            set
            {
                if (value) _queueService.Start();
                else _queueService.Stop();

                this.RaiseAndSetIfChanged(ref _queueRunning, value);
            }
        }

        #endregion

        #region Services

        private readonly QueueService _queueService;

        #endregion

        #region Commands

        

        #endregion
    }
}
