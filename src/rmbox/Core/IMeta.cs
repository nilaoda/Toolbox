using System;

namespace Ruminoid.Toolbox.Core
{
    public interface IMeta
    {
        public string Name { get; }

        public string Description { get; }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class RmboxPluginAttribute : Attribute
    {
    }
}
