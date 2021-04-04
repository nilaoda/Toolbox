using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using Ruminoid.Toolbox.Shell.Services;
using Ruminoid.Toolbox.Shell.ViewModels.Project;
using Ruminoid.Toolbox.Shell.Views;
using Splat;

namespace Ruminoid.Toolbox.Shell.ViewModels
{
    public class QueueViewModel : ReactiveObject
    {
        #region Constructor

        public QueueViewModel(
            QueueView queueView)
        {
            _queueView = queueView;

            _queueService = Locator.Current
                .GetService<QueueService>();

            _queueService
                .Connect()
                .Bind(out _items)
                .Subscribe();

            _queueService.RunnerOutput
                .Subscribe(NewRunnerOutput);

            _currentProject = this
                .WhenAnyValue(x => x._queueService.CurrentProject)
                .ToProperty(this, x => x.CurrentProject);

            _isAnyItem = this
                .WhenAnyValue(x => x.Items)
                .Any()
                .ToProperty(this, x => x.IsAnyItem);
        }

        #endregion

        #region View

        private readonly QueueView _queueView;
        
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

        #region Bindings

        private readonly ObservableAsPropertyHelper<bool> _isAnyItem;

        public bool IsAnyItem => _isAnyItem.Value;

        #endregion

        #region Runner Output

        private string _runnerOutput = "";

        public string RunnerOutput
        {
            get => _runnerOutput;
            set => this.RaiseAndSetIfChanged(ref _runnerOutput, value);
        }

        private int _caretIndex;

        public int CaretIndex
        {
            get => _caretIndex;
            set => this.RaiseAndSetIfChanged(ref _caretIndex, value);
        }

        private void NewRunnerOutput(string line)
        {
            RunnerOutput += line.Trim() + Environment.NewLine;

            while (RunnerOutput.Length > 10000)
            {
                var l = RunnerOutput.IndexOf(Environment.NewLine, StringComparison.Ordinal);
                if (l >= 0)
                    RunnerOutput = RunnerOutput[(l + Environment.NewLine.Length)..];
            }

            CaretIndex = RunnerOutput.Length;
        }

        public void DoClearRunnerOutput()
        {
            CaretIndex = 0;
            RunnerOutput = "";
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
