using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

// ReSharper disable MemberCanBePrivate.Global

namespace Ruminoid.Toolbox.Utils.Extensions
{
    public static partial class ProcessExtension
    {
        /// <summary>
        /// 运行工具进程。
        /// </summary>
        /// <param name="target">工具进程目标。</param>
        /// <param name="args">运行参数。</param>
        /// <returns>工具进程的输出。</returns>
        /// <exception cref="ProcessExtensionException">工具进程运行错误异常。</exception>
        public static string RunToolProcess(string target, string args)
        {
            string workingDirectory = StorageHelper.GetSectionFolderPath("tools");

            string targetPath = Path.Combine(
                workingDirectory,
                target + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : ""));

            return RunExternalProcess(
                targetPath,
                args,
                workingDirectory);
        }

        /// <summary>
        /// 运行外部进程。
        /// </summary>
        /// <param name="targetPath">外部进程目标。</param>
        /// <param name="args">运行参数。</param>
        /// <param name="workingDirectory"></param>
        /// <returns>外部进程的输出。</returns>
        /// <exception cref="ProcessExtensionException">外部进程运行错误异常。</exception>
        public static string RunExternalProcess(
            string targetPath,
            string args,
            string workingDirectory = null)
        {
            Process process = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    StandardErrorEncoding = Encoding.UTF8,
                    StandardInputEncoding = Encoding.UTF8,
                    StandardOutputEncoding = Encoding.UTF8,
                    WorkingDirectory = string.IsNullOrWhiteSpace(workingDirectory) ? AppDomain.CurrentDomain.BaseDirectory : workingDirectory,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    Arguments = ' ' + args,
                    FileName = targetPath,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };

            process.Start();

            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit(60 * 1000);

            if (!process.HasExited)
            {
                process.Kill(true);
                throw new ProcessExtensionException($"外部进程{targetPath}因为超时而被强制退出。");
            }

            if (process.ExitCode != 0)
            {
                throw new ProcessExtensionException($"外部进程{targetPath}错误退出，退出码为{process.ExitCode}。");
            }

            return result;
        }

        public static string GetPathExecutable() =>
            GetPathExecutable(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd" : "bash");

        public static string GetPathExecutable(string pathExecutable) =>
            RunExternalProcess(
                    RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                        ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "where.exe")
                        : "/usr/bin/which",
                    pathExecutable)
                .GetLines()
                .Where(x =>
                    RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Path.HasExtension(x) ||
                    !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                .FirstOrDefault(File.Exists);
    }

    [Serializable]
    public class ProcessExtensionException : Exception
    {
        public ProcessExtensionException()
        {
        }

        public ProcessExtensionException(string message) : base(message)
        {
        }

        public ProcessExtensionException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
