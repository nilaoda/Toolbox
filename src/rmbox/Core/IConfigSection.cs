using System;
using Avalonia.Styling;

namespace Ruminoid.Toolbox.Core
{
    public interface IConfigSection : ITemplatedControl
    {
        public object Config { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ConfigSectionAttribute : Attribute
    {
        public ConfigSectionAttribute(string name) => Name = name;

        public readonly string Name;
    }
}
