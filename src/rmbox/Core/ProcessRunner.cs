using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ruminoid.Toolbox.Composition;
using Ruminoid.Toolbox.Formatting;
using Ruminoid.Toolbox.Helpers.CommandLine;
using Ruminoid.Toolbox.Utils;

namespace Ruminoid.Toolbox.Core
{
    [Export]
    public class ProcessRunner : IDisposable
    {
        public ProcessRunner(
            CommandLineHelper commandLineHelper,
            PluginHelper pluginHelper,
            FormattingHelper formattingHelper,
            ILogger<ProcessRunner> logger)
        {
            _commandLineHelper = commandLineHelper;
            _pluginHelper = pluginHelper;
            _formattingHelper = formattingHelper;
            _logger = logger;

            int dynamicLinkPid = ((ProcessOptions) _commandLineHelper.Options).DynamicLinkPid;

            if (dynamicLinkPid != 0)
            {
                var pipe = new NamedPipeClientStream(
                    ".",
                    DynamicLinkPrefix + dynamicLinkPid,
                    PipeDirection.Out,
                    PipeOptions.Asynchronous);
                _pipeWriter = new StreamWriter(pipe);
            }

            _unsubscribe = _formattingHelper.FormatData.Subscribe(
                OnNext,
                OnError);
        }

        #region Dynamic Link

        public const string DynamicLinkPrefix = "RMBOXRNDYN";

        private readonly StreamWriter _pipeWriter;

        #endregion

        #region Runner

        /// <summary>
        /// 执行进程。
        /// </summary>
        /// <param name="target">进程目标。</param>
        /// <param name="args">进程参数。</param>
        public void Run(string target, string args)
        {
            _logger.LogDebug($"Resolving target: {target}");

            string workingDirectory = StorageHelper.GetSectionFolderPath("tools");

            string targetPath = Path.Combine(
                workingDirectory,
                target + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : ""));

            if (!File.Exists(targetPath))
            {
                string err = $"未能找到进程文件：{targetPath}。可能需要安装相关的插件以解决此问题。";
                _logger.LogCritical(err);
                throw new ProcessRunnerException(err);
            }

            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    StandardErrorEncoding = Encoding.UTF8,
                    StandardInputEncoding = Encoding.UTF8,
                    StandardOutputEncoding = Encoding.UTF8,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    Arguments = ' ' + args,
                    FileName = targetPath
                },
                EnableRaisingEvents = true
            };
            
            // ReSharper disable once InvokeAsExtensionMethod
            IDisposable observable = Observable.Merge(
                    Observable.FromEventPattern<DataReceivedEventArgs>(process, nameof(process.ErrorDataReceived)),
                    Observable.FromEventPattern<DataReceivedEventArgs>(process, nameof(process.OutputDataReceived)),
                    Scheduler.Default)
                .Subscribe(
                    next =>
                    {
                        var (_, e) = next;
                        if (string.IsNullOrEmpty(e.Data)) return;

                        if (((ProcessOptions) _commandLineHelper.Options).LogProcessOut)
                            _logger.LogInformation($"[{target}]{e.Data}");
                        _formattingHelper.ReceiveData.OnNext(new Tuple<string, string>(target, e.Data));
                    },
                    error => _logger.LogError(error, "进程发生了错误。"));

            _logger.LogInformation($"开始运行：{targetPath} {args}");
            
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit();

            observable.Dispose();

            if (process.ExitCode != 0)
            {
                string err = $"{target} 程序出现错误，退出码为 {process.ExitCode}。";
                _logger.LogCritical(err);
                throw new ProcessRunnerException(err);
            }

            _logger.LogInformation($"{target} 运行结束，程序正常退出。");
        }

        /// <summary>
        /// 执行进程。
        /// </summary>
        /// <param name="command">指令。</param>
        public void Run(Tuple<string, string> command) =>
            Run(command.Item1, command.Item2);

        /// <summary>
        /// 执行进程。
        /// </summary>
        /// <param name="commands">指令列表。</param>
        public void Run(List<Tuple<string, string>> commands)
        {
            _logger.LogInformation($"开始运行 {commands.Count} 条指令。");

            commands.ForEach(Run);
        }

        #endregion

        #region Subscribe

        private readonly IDisposable _unsubscribe;

        private void OnError(Exception error) => _logger.LogError(error, "进程发生了错误。");

        private void OnNext(FormattedEvent formatted)
        {
            _logger.LogInformation(formatted.ToString());
            _pipeWriter?.WriteLineAsync(JsonConvert.SerializeObject(formatted));
        }

        #endregion

        private readonly CommandLineHelper _commandLineHelper;
        private readonly PluginHelper _pluginHelper;
        private readonly FormattingHelper _formattingHelper;
        private readonly ILogger<ProcessRunner> _logger;

        public void Dispose()
        {
            _unsubscribe?.Dispose();
        }
    }
}
