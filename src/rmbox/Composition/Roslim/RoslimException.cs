using System;

namespace Ruminoid.Toolbox.Composition.Roslim
{
    [Serializable]
    public class RoslimException : Exception
    {
        public RoslimException()
        {
        }

        public RoslimException(string message) : base(message)
        {
        }

        public RoslimException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
