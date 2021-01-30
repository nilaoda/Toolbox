using System;
using Avalonia.Controls.Primitives;

namespace Ruminoid.Toolbox.Core
{
    public abstract class ConfigSectionBase : TemplatedControl
    {
        public string Header { get; set; }

        public object Config { get; set; }

        public string CurrentHelpText { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ConfigSectionAttribute : Attribute
    {
        public ConfigSectionAttribute(string id) => Id = id;

        public readonly string Id;
    }
}
