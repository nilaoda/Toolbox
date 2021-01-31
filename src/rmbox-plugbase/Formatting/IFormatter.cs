using System;

namespace Ruminoid.Toolbox.Formatting
{
    public interface IFormatter
    {
        public FormattedEvent Format(string target, string data);
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class FormatterAttribute : Attribute
    {
        public FormatterAttribute(string targets) => Targets = targets;

        public readonly string Targets;
    }
}
