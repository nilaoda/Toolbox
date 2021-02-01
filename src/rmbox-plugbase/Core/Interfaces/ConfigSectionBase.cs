using System;
using Avalonia.Controls;

namespace Ruminoid.Toolbox.Core
{
    public abstract class ConfigSectionBase : UserControl
    {
        public string Header { get; }

        public object Config { get; }

        public string CurrentHelpText { get; }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ConfigSectionAttribute : Attribute
    {
        public ConfigSectionAttribute(string id) => Id = id;

        public readonly string Id;
    }
}
