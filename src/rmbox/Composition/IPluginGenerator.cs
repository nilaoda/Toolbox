using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Ruminoid.Toolbox.Composition
{
    public interface IPluginGenerator
    {
        public (JObject Meta, Assembly Assembly) Generate(string path);
    }
}
