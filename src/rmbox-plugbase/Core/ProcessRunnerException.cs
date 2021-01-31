using System;

namespace Ruminoid.Toolbox.Core
{
    [Serializable]
    public class ProcessRunnerException : Exception
    {
        public ProcessRunnerException()
        {
        }

        public ProcessRunnerException(string message) : base(message)
        {
        }

        public ProcessRunnerException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
