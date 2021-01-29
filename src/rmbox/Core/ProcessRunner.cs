using System;
using System.Composition;

namespace Ruminoid.Toolbox.Core
{
    [Export]
    public class ProcessRunner
    {
        /// <summary>
        /// 执行进程。
        /// </summary>
        /// <param name="target">进程目标。</param>
        /// <param name="args">进程参数。</param>
        /// <returns>进程是否成功执行。</returns>
        public bool Run(string target, string args)
        {
            throw new NotImplementedException();
        }
    }
}
