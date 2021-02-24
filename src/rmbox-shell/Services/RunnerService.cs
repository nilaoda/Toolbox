﻿using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;
using ReactiveUI;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Formatting;
using Ruminoid.Toolbox.Shell.Core;
using Ruminoid.Toolbox.Shell.ViewModels;
using Ruminoid.Toolbox.Utils;

namespace Ruminoid.Toolbox.Shell.Services
{
    internal class RunnerService
    {
        #region Constructor

        public RunnerService(
            QueueService queueService)
        {
            _queueService = queueService;

            NamedPipeServerStream pipe =
                new NamedPipeServerStream(
                    ProcessRunner.DynamicLinkPrefix + Environment.ProcessId,
                    PipeDirection.In,
                    NamedPipeServerStream.MaxAllowedServerInstances,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous | PipeOptions.WriteThrough);

            Observable.FromAsync(token => pipe.WaitForConnectionAsync(token))
                .Subscribe(_ =>
                {
                    Observable.Using(
                            () => new StreamReader(pipe),
                            reader =>
                                Observable.FromAsync(reader.ReadLineAsync)
                                    .Repeat()
                                    .Where(x => !string.IsNullOrWhiteSpace(x)))
                        .Subscribe(ReadFromPipe);
                });

            _queueService
                .WhenAnyValue(x => x.CurrentProject)
                .Where(x => x is not null)
                .Where(_ => _queueService.QueueRunning)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Subscribe(Run);
        }

        #endregion

        #region Data

        private ProjectViewModel CurrentProject => _queueService.CurrentProject;

        private readonly Subject<object> _killSubject = new();

        #endregion

        #region Dynamic Link
        
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
                    Arguments = $" \"{path}\" -o -d {Environment.ProcessId}",
                    FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                        "rmbox" + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : ""))
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

            bool killFlag = false;

            IDisposable killObservable = _killSubject.Subscribe(_ =>
            {
                killFlag = true;
                process.Kill();
                process.Dispose();
            });

            process.WaitForExit();

            killObservable.Dispose();
            observable.Dispose();

            bool succeed;

            // ReSharper disable once InvertIf
            if (killFlag)
            {
                CurrentProject.Detail = "运行被取消。";
                succeed = false;
            }
            else
                succeed = process.ExitCode == 0;

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
