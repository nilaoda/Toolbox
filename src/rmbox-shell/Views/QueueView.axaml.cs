using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using DynamicData;
using JetBrains.Annotations;
using ReactiveUI;
using Ruminoid.Common2.Metro.MetroControls.Dialogs;
using Ruminoid.Toolbox.Shell.Services;
using Ruminoid.Toolbox.Shell.ViewModels.Project;
using Splat;

namespace Ruminoid.Toolbox.Shell.Views
{
    public class QueueView : UserControl
    {
        public QueueView()
        {
            DataContext = new QueueViewModel(this);

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    public partial class QueueViewModel : ReactiveObject
    {
        #region Constructor

        public QueueViewModel(
            QueueView view)
        {
            _view = view;

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

            #region Status

            this
                .ObservableForProperty(x => x.Items)
                .Subscribe(_ => TriggerStatusUpdate());

            this
                .ObservableForProperty(x => x._queueService.CurrentProject)
                .Subscribe(_ => TriggerStatusUpdate());

            this
                .ObservableForProperty(x => x._queueService.CurrentProjectStatus)
                .Subscribe(_ => TriggerStatusUpdate());

            this
                .ObservableForProperty(x => x._queueService.QueueRunning)
                .Subscribe(_ => TriggerStatusUpdate());

            _statusSummary = this
                .WhenAnyValue(x => x.CurrentStatus)
                .Select(x => x switch
                {
                    QueueStatus.Ready => "就绪",
                    QueueStatus.Free => "空闲",
                    QueueStatus.Running => "运行",
                    QueueStatus.RunningWithoutQueue => "运行（队列暂停）",
                    QueueStatus.Error => "错误",
                    _ => throw new ArgumentOutOfRangeException(nameof(x), x, null)
                })
                .ToProperty(this, x => x.StatusSummary);

            _statusDetail = this
                .WhenAnyValue(x => x.CurrentStatus)
                .Select(x => x switch
                {
                    QueueStatus.Ready => "队列已经启动。新的任务加入队列后，队列将立即开始运行。",
                    QueueStatus.Free => "队列未启动，列表中的任务不会被运行。若要启动队列，请轻敲下方的「启动」按钮启动队列。",
                    QueueStatus.Running => "队列正在运行中。任务运行完成后将立即运行下一个任务。",
                    QueueStatus.RunningWithoutQueue => "任务正在运行中，但队列暂停。任务运行完成后将等待指令。",
                    QueueStatus.Error => "运行遇到错误，因此等待指令。\n尝试按照下面的步骤排查问题：\n1. 检视右侧的「日志」视口以确定问题详细信息。\n2. 若问题已经解决，请轻敲下方的「跳过」按钮恢复队列。",
                    _ => throw new ArgumentOutOfRangeException(nameof(x), x, null)
                })
                .ToProperty(this, x => x.StatusDetail);

            #endregion
        }

        #endregion

        #region View

        private readonly QueueView _view;

        #endregion

        #region Data

        private readonly ReadOnlyObservableCollection<ProjectViewModel> _items;

        [UsedImplicitly]
        public ReadOnlyObservableCollection<ProjectViewModel> Items => _items;

        private readonly ObservableAsPropertyHelper<ProjectViewModel> _currentProject;

        [UsedImplicitly]
        public ProjectViewModel CurrentProject => _currentProject.Value;

        #endregion

        #region Runner Output

        private string _runnerOutput = "";

        [UsedImplicitly]
        public string RunnerOutput
        {
            get => _runnerOutput;
            set => this.RaiseAndSetIfChanged(ref _runnerOutput, value);
        }

        private int _caretIndex;

        [UsedImplicitly]
        public int CaretIndex
        {
            get => _caretIndex;
            set => this.RaiseAndSetIfChanged(ref _caretIndex, value);
        }

        private bool _scrollLocked;

        [UsedImplicitly]
        public bool ScrollLocked
        {
            get => _scrollLocked;
            set => this.RaiseAndSetIfChanged(ref _scrollLocked, value);
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

            if (!ScrollLocked) CaretIndex = RunnerOutput.Length;
        }

        #endregion

        #region Services

        private readonly QueueService _queueService;

        #endregion

        #region Commands

        [UsedImplicitly]
        public void DoToggle()
        {
            if (_queueService.QueueRunning) _queueService.Stop();
            else _queueService.Start();
        }

        [UsedImplicitly]
        public void DoKill() => _queueService.Kill();

        [UsedImplicitly]
        public void DoClear() => _queueService.Clear();

        [UsedImplicitly]
        public void DoSkip() => _queueService.Skip();

        [UsedImplicitly]
        public async Task DoSaveRunnerOutput()
        {
            SaveFileDialog dialog = new()
            {
                Title = "保存日志",
                DefaultExtension = "log",
                InitialFileName = $"rmbox-log-{DateTime.Now:MM-dd-hh-mm-ss}.log",
                Filters = new()
                {
                    new() {Name = "日志文件", Extensions = new() {"log"}},
                    new() {Name = "文本文件", Extensions = new() {"txt"}}
                }
            };

            string result = await dialog.ShowAsync((Window) _view.GetVisualRoot());

            if (string.IsNullOrWhiteSpace(result)) return;

            try
            {
                await File.WriteAllTextAsync(
                    result,
                    RunnerOutput);
            }
            catch (Exception)
            {
                await MessageBox.ShowAndGetResult(
                    "错误",
                    "导出日志时发生了错误。",
                    (Window) _view.GetVisualRoot(),
                    false);
            }
        }

        [UsedImplicitly]
        public void DoClearRunnerOutput()
        {
            CaretIndex = 0;
            RunnerOutput = "";
        }

        #endregion
    }
}
