using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Utils.Extensions;

namespace Ruminoid.Toolbox.Plugins.Audio.Operations
{
    [Operation(
        "Ruminoid.Toolbox.Plugins.Audio.Operations.AudioEncodeOperation",
        "音频压制",
        "使用 QAAC 压制音频。")]
    public class AudioEncodeOperation : IOperation
    {
        public List<(string Target, string Args, string Formatter)> Generate(Dictionary<string, JToken> sectionData)
        {
            JToken ioSection =
                sectionData[ConfigSectionBase.IOConfigSectionId];
            JToken audioSection =
                sectionData["Ruminoid.Toolbox.Plugins.Audio.ConfigSections.AudioQualityConfigSection"];

            string inputPath = PathExtension.GetFullPathOrEmpty(ioSection["input"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();
            string outputPath = PathExtension.GetFullPathOrEmpty(ioSection["output"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();

            int audioBitrate = audioSection["bitrate"].ToObject<int>();

            return new List<(string, string, string)>
            {
                new(
                    "qaac64",
                    $" -q 2 --ignorelength -c {audioBitrate} {inputPath} -o {outputPath}",
                    "null")
            };
        }

        public Dictionary<string, JToken> RequiredConfigSections => new()
        {
            {ConfigSectionBase.IOConfigSectionId, new JObject()},
            {"Ruminoid.Toolbox.Plugins.Audio.ConfigSections.AudioQualityConfigSection", new JObject()}
        };
    }
}
