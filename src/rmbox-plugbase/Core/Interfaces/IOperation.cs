using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Ruminoid.Common2.Utils.UserTypes;

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
        public OperationAttribute(
            string id,
            string name,
            string description,
            RateValue rate,
            string category)
        {
            Id = id;
            Name = name;
            Description = description;
            Rate = rate;
            Category = category;
        }

        public readonly string Id;

        public readonly string Name;

        public readonly string Description;

        public readonly Rate Rate;

        public readonly string Category;
    }
}
