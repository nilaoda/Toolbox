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
        public List<(string Target, string Args)> Generate(Dictionary<string, JToken> sectionData)
        {
            string sectionDataPath = StorageHelper.GetSectionFilePath("temp", $"secdat-{Guid.NewGuid()}.json");

            File.WriteAllText(sectionDataPath, JsonConvert.SerializeObject(sectionData));

            try
            {
                string raw = ExternalProcessRunner.Run("target", $"\"script\" \"{sectionDataPath}\"");
                return JArray.Parse(raw)
                    .Select(x => (x[ /* MAGIC */ "tar" + "get"].ToObject<string>(), x["args"].ToObject<string>()))
                    .ToList();
            }
            finally
            {
                if (File.Exists(sectionDataPath)) File.Delete(sectionDataPath);
            }
        }

        public Dictionary<string, JToken> RequiredConfigSections => new()
        {
            {"Ruminoid.Toolbox.Composition.Roslim.RoslimConfigSection", new JObject()}
        };
    }
}
