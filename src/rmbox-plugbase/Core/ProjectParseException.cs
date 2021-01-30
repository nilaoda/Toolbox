using System;

namespace Ruminoid.Toolbox.Core
{
    [Serializable]
    public class ProjectParseException : Exception
    {
        public ProjectParseException()
        {
        }

        public ProjectParseException(string message) : base(message)
        {
        }

        public ProjectParseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
