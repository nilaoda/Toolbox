using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Utils;

namespace Ruminoid.Toolbox.Composition.Roslim
{
    [Operation(
        "Ruminoid.Toolbox.Composition.Roslim.RoslimDefaultOperation",
        "(Roslim)",
        "(Roslim Operation)")]
    public class RoslimOperation : IOperation
    {
        public List<Tuple<string, string>> Generate(Dictionary<string, JToken> sectionData)
        {
            string sectionDataPath = StorageHelper.GetSectionFilePath("temp", $"secdat-{Guid.NewGuid()}.json");

            File.WriteAllText(sectionDataPath, JsonConvert.SerializeObject(sectionData));

            try
            {
                string result = ExternalProcessRunner.Run("target", $"\"script\" \"{sectionDataPath}\"");
                List<JToken> commands = JArray.Parse(result).ToObject<List<JToken>>();
                return commands!.Select(x =>
                    new Tuple<string, string>(x["target"].ToObject<string>(), x["args"].ToObject<string>()))
                    .ToList();
            }
            finally
            {
                if (File.Exists(sectionDataPath)) File.Delete(sectionDataPath);
            }
        }

        public List<string> RequiredConfigSections => new()
        {
            "Ruminoid.Toolbox.Composition.Roslim.RoslimConfigSection"
        };
    }
}
