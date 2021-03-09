using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Fleck;
using Newtonsoft.Json;
using ReactiveUI;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Formatting;
using Ruminoid.Toolbox.Shell.Core;
using Ruminoid.Toolbox.Shell.ViewModels;
using Ruminoid.Toolbox.Utils;
using Ruminoid.Toolbox.Utils.Extensions;

namespace Ruminoid.Toolbox.Shell.Services
{
    internal class RunnerService
    {
        #region Constructor

        public RunnerService(
            QueueService queueService)
        {
            _queueService = queueService;

            _dynamicLink.Subscribe(ReadFromPipe);

            WebSocketServer webSocketServer = new($"ws://127.0.0.1:{_pipePort}")
            {
                RestartAfterListenError = true
            };

            webSocketServer.Start(socket =>
            {
                IDisposable killDisposable = _killSubject.Subscribe(_ =>
                {
                    socket.Send(ProcessRunner.DynamicLinkKillCommand);
                });

                socket.OnMessage = message => _dynamicLink.OnNext(message);
                socket.OnError = ex => _dynamicLink.OnError(ex);

                socket.OnClose = () =>
                {
                    killDisposable.Dispose();
                    _dynamicLink.OnCompleted();
                };
            });

            _queueService
                .WhenAnyValue(x => x.CurrentProject)
                .Where(x => x is not null)
                .Where(_ => _queueService.QueueRunning)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Subscribe(Run);

            _queueService
                .WhenAnyValue(x => x.QueueRunning)
                .Where(x => x)
                .Where(_ => CurrentProject is not null)
                .Where(_ => CurrentProject.Status == ProjectStatus.Queued)
                .Select(_ => CurrentProject)
                .Subscribe(Run);
        }

        #endregion

        #region Data

        private ProjectViewModel CurrentProject => _queueService.CurrentProject;

        private readonly Subject<object> _killSubject = new();

        #endregion

        #region Dynamic Link

        private readonly int _pipePort = new Random().Next(30010, 31000);

        private readonly Subject<string> _dynamicLink = new();

        private void ReadFromPipe(string json)
        {
            if (CurrentProject is null) return;
            try
            {
                FormattedEvent formattedEvent = JsonConvert.DeserializeObject<FormattedEvent>(json);

                CurrentProject.Progress = formattedEvent.Progress;
                CurrentProject.IsIndeterminate = formattedEvent.IsIndeterminate;
                CurrentProject.Summary = formattedEvent.Summary;
                CurrentProject.Detail = formattedEvent.Detail;
            }
            catch (JsonException)
            {
                // Ignore
            }
        }

        #endregion

        #region Services

        private readonly QueueService _queueService;

        #endregion

        #region Operations
        
        private void Run(ProjectViewModel project)
        {
            project.Status = ProjectStatus.Running;
            project.IsIndeterminate = true;
            project.Summary = "准备生成项目";

            CurrentProject.IsIndeterminate = true;
            CurrentProject.Summary = "生成项目文件";
            
            string path = StorageHelper.GetSectionFilePath("temp", $"proj-{CurrentProject.Id}.json");
            Exporter.ExportProjectToFile(project, path);

            CurrentProject.Summary = "准备启动运行";

            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    StandardErrorEncoding = Encoding.UTF8,
                    StandardInputEncoding = Encoding.UTF8,
                    StandardOutputEncoding = Encoding.UTF8,
                    WorkingDirectory = StorageHelper.GetSectionFolderPath("tools"),
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    Arguments = $" {path.EscapePathStringForArg()} -o -h -d {_pipePort}",
                    FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                        "rmbox" + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : "")),
                    WindowStyle = ProcessWindowStyle.Hidden
                },
                EnableRaisingEvents = true
            };

            // ReSharper disable once InvokeAsExtensionMethod
            IDisposable observable = Observable
                .Merge(
                    Observable.FromEventPattern<DataReceivedEventArgs>(process, nameof(process.ErrorDataReceived)),
                    Observable.FromEventPattern<DataReceivedEventArgs>(process, nameof(process.OutputDataReceived)))
                .Subscribe(next =>
                {
                    var (_, e) = next;
                    if (string.IsNullOrEmpty(e.Data)) return;
                    _queueService.RunnerOutput.OnNext(e.Data);
                });

            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit();
            
            observable.Dispose();
            
            var succeed = process.ExitCode == 0;

            CurrentProject.Summary = "清理";

            if (File.Exists(path)) File.Delete(path);

            CurrentProject.IsIndeterminate = false;
            CurrentProject.Progress = 100;
            CurrentProject.Summary = succeed ? "完成" : "错误";
            CurrentProject.Status = succeed ? ProjectStatus.Completed : ProjectStatus.Error;
        }

        internal void Kill() => _killSubject.OnNext(null);

        #endregion
    }
}
