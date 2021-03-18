using System;

namespace Ruminoid.Toolbox.Core
{
    public interface IMeta
    {
        public string Name { get; }

        public string Description { get; }

        public string Author { get; }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class RmboxPluginAttribute : Attribute
    {
    }
}
