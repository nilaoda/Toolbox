using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Utils.Extensions;

namespace Ruminoid.Toolbox.Plugins.HwEnc.Operations
{
    [Operation(
        "Ruminoid.Toolbox.Plugins.HwEnc.Operations.HwEncOperation",
        "显卡压制",
        "使用显卡进行视频压制。")]
    public class HwEncOperation : IOperation
    {
        public List<(string Target, string Args)> Generate(Dictionary<string, JToken> sectionData)
        {
            JToken ioSection =
                sectionData["Ruminoid.Toolbox.Plugins.Common.ConfigSections.IOConfigSection"];
            JToken hwEncQualitySection =
                sectionData["Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.HwEncQualityConfigSection"];
            JToken hwEncCodecSection =
                sectionData["Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.HwEncCodecConfigSection"];
            JToken hwEncCoreSection =
                sectionData["Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.HwEncCoreConfigSection"];
            JToken customArgsSection =
                sectionData["Ruminoid.Toolbox.Plugins.Common.ConfigSections.CustomArgsConfigSection"];

            string hwEncCore = hwEncCoreSection["core"]?.ToObject<string>();

            string customArgs = customArgsSection["args"]?.ToObject<string>();
            bool useCustomArgs = !string.IsNullOrWhiteSpace(customArgs);

            string defaultArgs = "";

            string videoPathIntl = Path.GetFullPath(ioSection["video"]?.ToObject<string>() ?? string.Empty);
            string videoPath = videoPathIntl.EscapePathStringForArg();
            string outputPath = Path.GetFullPath(ioSection["output"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();

            switch (hwEncQualitySection["encode_mode"]?.ToObject<string>())
            {
                case "cqp":
                    return new List<(string Target, string Args)>
                    {
                        (hwEncCore,
                            $"-i {videoPath} -o {outputPath} --avhw --audio-copy --codec {hwEncCodecSection["codec"]?.ToObject<string>()} --cqp {hwEncQualitySection["cqp_value"]?.ToObject<string>()} {(useCustomArgs ? customArgs : defaultArgs)}")
                    };
                case "2pass":
                    return new List<(string Target, string Args)>
                    {
                        (hwEncCore,
                            $"-i {videoPath} -o {outputPath} --avhw --audio-copy --codec {hwEncCodecSection["codec"]?.ToObject<string>()} --vbr {hwEncQualitySection["2pass_value"]?.ToObject<int>()} --multipass 2pass-full {(useCustomArgs ? customArgs : defaultArgs)}")
                    };
                default:
                    throw new ArgumentOutOfRangeException("encode_mode");
            }
        }

        public SortedDictionary<string, JToken> RequiredConfigSections
        {
            get
            {
                SortedDictionary<string, JToken> sections = new();

                sections.Add("Ruminoid.Toolbox.Plugins.Common.ConfigSections.IOConfigSection", new JObject());
                sections.Add("Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.HwEncCoreConfigSection", new JObject());
                sections.Add("Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.HwEncCodecConfigSection", new JObject());
                sections.Add("Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.HwEncQualityConfigSection", new JObject());
                sections.Add("Ruminoid.Toolbox.Plugins.Common.ConfigSections.CustomArgsConfigSection", new JObject());

                return sections;
            }
        }
    }
}
