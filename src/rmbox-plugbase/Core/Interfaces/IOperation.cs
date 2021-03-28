using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Ruminoid.Toolbox.Core
{
    public interface IOperation
    {
        public List<TaskCommand> Generate(Dictionary<string, JToken> sectionData);

        public Dictionary<string, JToken> RequiredConfigSections { get; }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class OperationAttribute : Attribute
    {
        public OperationAttribute(string id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public readonly string Id;

        public readonly string Name;

        public readonly string Description;
    }
}
