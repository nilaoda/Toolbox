using System;
using System.Collections.Generic;

namespace Ruminoid.Toolbox.Formatting
{
    public interface IFormatter
    {
        public FormattedEvent Format(string target, string data, Dictionary<string, object> sessionStorage);
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class FormatterAttribute : Attribute
    {
        public FormatterAttribute(string targets) => Targets = targets;

        public readonly string Targets;
    }
}
