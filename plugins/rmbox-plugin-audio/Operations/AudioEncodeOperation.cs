using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Ruminoid.Common2.Utils.UserTypes;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Utils.Extensions;

namespace Ruminoid.Toolbox.Plugins.Audio.Operations
{
    [Operation(
        "Ruminoid.Toolbox.Plugins.Audio.Operations.AudioEncodeOperation",
        "音频压制",
        "使用 QAAC 压制音频。",
        RateValue.ThreeStars,
        "音频压制")]
    public class AudioEncodeOperation : IOperation
    {
        public List<TaskCommand> Generate(Dictionary<string, JToken> sectionData)
        {
            JToken ioSection =
                sectionData[ConfigSectionBase.IOConfigSectionId];
            JToken audioSection =
                sectionData["Ruminoid.Toolbox.Plugins.Audio.ConfigSections.AudioQualityConfigSection"];

            string inputPath = PathExtension.GetFullPathOrEmpty(ioSection["input"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();
            string outputPath = PathExtension.GetFullPathOrEmpty(ioSection["output"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();

            int audioBitrate = audioSection["bitrate"].ToObject<int>();

            #region 自定义参数

            JToken customArgsSection =
                sectionData["Ruminoid.Toolbox.Plugins.Common.ConfigSections.CustomArgsConfigSection"];

            string customArgs = customArgsSection["args"]?.ToObject<string>();
            bool useCustomArgs = customArgsSection["use_custom_args"]?.ToObject<bool>() ?? false;

            #endregion

            return new List<TaskCommand>
            {
                new(
                    "qaac64",
                    $" -q 2 --ignorelength -c {audioBitrate} {(useCustomArgs ? customArgs : DefaultArgs)} {inputPath} -o {outputPath}",
                    "null")
            };
        }

        private const string DefaultArgs = "";

        public Dictionary<string, JToken> RequiredConfigSections => new()
        {
            {
                ConfigSectionBase.IOConfigSectionId,
                JToken.FromObject(new
                {
                    output_suffix = "_encoded",
                    output_extension = ".aac"
                })
            },
            {"Ruminoid.Toolbox.Plugins.Audio.ConfigSections.AudioQualityConfigSection", new JObject()},
            {
                "Ruminoid.Toolbox.Plugins.Common.ConfigSections.CustomArgsConfigSection",
                JObject.FromObject(new
                {
                    default_args = DefaultArgs
                })
            }
        };
    }
}
