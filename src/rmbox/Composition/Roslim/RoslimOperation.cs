using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Utils;
using Ruminoid.Toolbox.Utils.Extensions;

namespace Ruminoid.Toolbox.Composition.Roslim
{
    [Operation(
        "Ruminoid.Toolbox.Composition.Roslim.RoslimDefaultOperation",
        "(Roslim)",
        "(Roslim Operation)",
        ROSLIM_OPERATION_RATE,
        "(Roslim Operation Category)")]
    public class RoslimOperation : IOperation
    {
        public List<TaskCommand> Generate(Dictionary<string, JToken> sectionData)
        {
            string sectionDataPath = StorageHelper.GetSectionFilePath("temp", $"secdat-{Guid.NewGuid()}.json");

            File.WriteAllText(sectionDataPath, JsonConvert.SerializeObject(sectionData));

            try
            {
                string raw = ProcessExtension.RunToolProcess("target", $"\"script\" \"{sectionDataPath}\"");
                return JArray.Parse(raw)
                    .Select<JToken, TaskCommand>(x =>
                        (x[ /* MAGIC */ "tar" + "get"].ToObject<string>(), x["args"].ToObject<string>(),
                            x["formatter"].ToObject<string>()))
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
