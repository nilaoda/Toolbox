using System;

namespace Ruminoid.Toolbox.Formatting
{
    public interface IFormatter
    {
        public FormattedEvent Format(string data);
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    sealed class FormatterAttribute : Attribute
    {
        public FormatterAttribute(string targets) => Targets = targets;

        public readonly string Targets;
    }
}
