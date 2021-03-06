﻿using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Ruminoid.Toolbox.Utils
{
    public static class ExternalProcessRunner
    {
        /// <summary>
        /// 运行外部进程。
        /// </summary>
        /// <param name="target">外部进程目标。</param>
        /// <param name="args">运行参数。</param>
        /// <returns>外部进程的输出。</returns>
        /// <exception cref="ExternalProcessRunnerException">外部进程运行错误异常。</exception>
        public static string Run(string target, string args)
        {
            string workingDirectory = StorageHelper.GetSectionFolderPath("tools");

            string targetPath = Path.Combine(
                workingDirectory,
                target + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : ""));

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
                throw new ExternalProcessRunnerException($"外部进程{target}因为超时而被强制退出。");
            }

            if (process.ExitCode != 0)
            {
                throw new ExternalProcessRunnerException($"外部进程{target}错误退出，退出码为{process.ExitCode}。");
            }

            return result;
        }
    }

    [Serializable]
    public class ExternalProcessRunnerException : Exception
    {
        public ExternalProcessRunnerException()
        {
        }

        public ExternalProcessRunnerException(string message) : base(message)
        {
        }

        public ExternalProcessRunnerException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}