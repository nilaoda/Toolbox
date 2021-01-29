using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Ruminoid.Toolbox.Core
{
    public interface IOperation
    {
        public List<KeyValuePair<string, string>> Generate(JObject config);

        public List<string> RequiredConfigSections { get; }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class OperationAttribute : Attribute
    {
        public OperationAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public readonly string Name;

        public readonly string Description;
    }
}
