using System;
using Avalonia.Controls;

namespace Ruminoid.Toolbox.Core
{
    public abstract class ConfigSectionBase : UserControl
    {
        public abstract object Config { get; }

        public string CurrentHelpText { get; }

        #region Consts

        // ReSharper disable once InconsistentNaming
        public const string IOConfigSectionId = "Ruminoid.Toolbox.Plugins.Common.ConfigSections.IOConfigSection";

        #endregion
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ConfigSectionAttribute : Attribute
    {
        public ConfigSectionAttribute(
            string id,
            string name)
        {
            Id = id;
            Name = name;
        }

        public readonly string Id;

        public readonly string Name;
    }
}
