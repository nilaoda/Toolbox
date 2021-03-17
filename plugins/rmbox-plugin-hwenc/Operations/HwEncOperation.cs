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
        public List<(string Target, string Args, string Formatter)> Generate(Dictionary<string, JToken> sectionData)
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

            string inputPathIntl = PathExtension.GetFullPathOrEmpty(ioSection["input"]?.ToObject<string>() ?? string.Empty);
            string inputPath = inputPathIntl.EscapePathStringForArg();
            string subtitlePathIntl = PathExtension.GetFullPathOrEmpty(ioSection["subtitle"]?.ToObject<string>() ?? string.Empty);
            string subtitlePath = subtitlePathIntl.EscapePathStringForArg();
            string outputPath = PathExtension.GetFullPathOrEmpty(ioSection["output"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();
            string lwiPath = (inputPathIntl + ".lwi").EscapePathStringForArg();

            bool isVpy = inputPathIntl.EndsWith(".vpy");
            bool isVsfm = !string.IsNullOrEmpty(subtitlePathIntl);

            bool useVpy = isVpy || isVsfm;

            if (isVpy && isVsfm)
                throw new OperationException(
                    "不支持在 VapourSynth 输入上使用 VSFilterMod。请在 VapourSynth 中完成字幕处理。",
                    typeof(HwEncOperation));

            List<(string Target, string Args, string Formatter)> result = new();

            if (isVsfm)
            {
                string vpyPath = Path.ChangeExtension(inputPathIntl, "vpy").EscapePathStringForArg();

                result.Add(
                    ("node",
                        $"rmbox-vpygen.js {inputPath} {subtitlePath} {vpyPath}",
                        "null"));

                inputPath = vpyPath;
            }

            string encodeMode = hwEncQualitySection["encode_mode"]?.ToObject<string>() switch
            {
                "cqp" => $"--cqp {hwEncQualitySection["cqp_value"]?.ToObject<string>()}",
                "2pass" => $"--vbr {hwEncQualitySection["2pass_value"]?.ToObject<int>()} --multipass 2pass-full",
                // ReSharper disable once NotResolvedInText
                _ => throw new ArgumentOutOfRangeException("encode_mode")
            };

            result.Add(
                (hwEncCore,
                    $"{(useVpy ? "--vpy" : "--avhw")} -i {inputPath} -o {outputPath} --audio-copy --codec {hwEncCodecSection["codec"]?.ToObject<string>()} {encodeMode} {(useCustomArgs ? customArgs : defaultArgs)}",
                    hwEncCore));

            if (useVpy)
            {
                result.Add(
                    ("pwsh",
                        "-Command If (Test-Path " + lwiPath + " ) { Remove-Item " + lwiPath + " }",
                        "null"));
                result.Add(
                    ("pwsh",
                        $"-Command Remove-Item {inputPath}",
                        "null"));
            }

            return result;
        }

        public Dictionary<string, JToken> RequiredConfigSections => new()
        {
            {
                "Ruminoid.Toolbox.Plugins.Common.ConfigSections.IOConfigSection", JToken.FromObject(new
                {
                    support_subtitle = true,
                    support_vsfm = true
                })
            },
            {"Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.HwEncCoreConfigSection", new JObject()},
            {"Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.HwEncCodecConfigSection", new JObject()},
            {"Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.HwEncQualityConfigSection", new JObject()},
            {"Ruminoid.Toolbox.Plugins.Common.ConfigSections.CustomArgsConfigSection", new JObject()}
        };
    }
}
