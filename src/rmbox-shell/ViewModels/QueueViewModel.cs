using System;
using System.Collections.ObjectModel;
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
                .Bind(out _items)
                .Subscribe();

            _queueService.RunnerOutput
                .Subscribe(NewRunnerOutput);

            this
                .WhenAnyValue(x => x._queueService.CurrentProject)
                .ToProperty(this, x => x.CurrentProject, out _currentProject);
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

        private readonly ObservableAsPropertyHelper<ProjectViewModel> _currentProject;

        public ProjectViewModel CurrentProject => _currentProject.Value;

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

        #region Runner Output

        //public ObservableCollection<string> RunnerOutputList { get; } = new();

        private string _currentRunnerOutputLine = "";

        public string CurrentRunnerOutputLine
        {
            get => _currentRunnerOutputLine;
            set => this.RaiseAndSetIfChanged(ref _currentRunnerOutputLine, value);
        }

        private void NewRunnerOutput(string line)
        {
            //RunnerOutputList.Add(line);
            //if (RunnerOutputList.Count > 100) RunnerOutputList.RemoveAt(0);
            CurrentRunnerOutputLine = line;
        }

        #endregion

        #region Services

        private readonly QueueService _queueService;

        #endregion

        #region Commands

        public void DoKill() => _queueService.Kill();

        public void DoClear() => _queueService.Clear();

        public void DoSkip() => _queueService.Skip();

        #endregion
    }
}
