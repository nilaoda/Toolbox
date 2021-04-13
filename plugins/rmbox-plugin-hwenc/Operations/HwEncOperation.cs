using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public List<TaskCommand> Generate(Dictionary<string, JToken> sectionData)
        {
            #region 输入/输出

            JToken ioSection =
                sectionData[ConfigSectionBase.IOConfigSectionId];

            #endregion

            #region 输入文件

            string inputPathIntl = PathExtension.GetFullPathOrEmpty(ioSection["input"]?.ToObject<string>() ?? string.Empty);
            string inputPath = inputPathIntl.EscapePathStringForArg();

            #endregion

            #region 输入字幕

            string subtitlePathIntl = PathExtension.GetFullPathOrEmpty(ioSection["subtitle"]?.ToObject<string>() ?? string.Empty);
            string subtitlePath = subtitlePathIntl.EscapePathStringForArg();

            #endregion

            #region 字幕相关

            bool isIncludingSubtitle = !string.IsNullOrWhiteSpace(subtitlePathIntl);

            #endregion

            #region VapourSynth 相关

            string lwiPath = (inputPathIntl + ".lwi").EscapePathStringForArg();

            bool isVpy = inputPathIntl.EndsWith(".vpy");
            bool isVsfm = isIncludingSubtitle;

            bool useVpy = isVpy || isVsfm;

            if (isVpy && isVsfm)
                throw new OperationException(
                    "不支持在 VapourSynth 输入上使用 VSFilterMod。请在 VapourSynth 中完成字幕处理。",
                    typeof(HwEncOperation));

            #endregion

            #region 显卡相关

            JToken hwEncQualitySection =
                sectionData["Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.HwEncQualityConfigSection"];
            JToken hwEncCodecSection =
                sectionData["Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.HwEncCodecConfigSection"];
            JToken hwEncCoreSection =
                sectionData["Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.HwEncCoreConfigSection"];

            string hwEncCore = hwEncCoreSection["core"]?.ToObject<string>();

            string vtempPath = Path.ChangeExtension(inputPathIntl, "vtemp.mp4").EscapePathStringForArg();

            #endregion

            #region 音频

            JToken audioSection =
                sectionData["Ruminoid.Toolbox.Plugins.Audio.ConfigSections.AudioConfigSection"];

            #endregion

            #region 音频相关

            string atempPath = Path.ChangeExtension(inputPathIntl, "atemp.mp4").EscapePathStringForArg();

            string audioMode = audioSection["mode"]?.ToObject<string>();

            //bool hasAudio = audioMode != "none";
            //int audioBitrate = audioSection["bitrate"].ToObject<int>();

            string audioArgs = audioMode switch
            {
                "process" => "--audio-codec --audio-bitrate " + audioSection["bitrate"]?.ToObject<int>(),
                "none" => "",
                _ => "--audio-copy"
            };

            #endregion

            #region 输出相关

            string outputPath = PathExtension.GetFullPathOrEmpty(ioSection["output"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();

            #endregion

            #region 自定义参数

            JToken customArgsSection =
                sectionData["Ruminoid.Toolbox.Plugins.Common.ConfigSections.CustomArgsConfigSection"];

            string customArgs = customArgsSection["args"]?.ToObject<string>();
            bool useCustomArgs = customArgsSection["use_custom_args"]?.ToObject<bool>() ?? false;

            #endregion

            #region 准备命令

            List<TaskCommand> result = new();

            #endregion

            // 开始处理

            #region 处理音频

            if (isVsfm)
                result.Add(new(
                    "ffmpeg",
                    $"-i {inputPath} -vn -sn -c:a copy -y -map 0:a:0 {atempPath}",
                    "null"));

            #endregion

            #region 处理 VSFM

            if (isVsfm)
            {
                string vpyPath = Path.ChangeExtension(inputPathIntl, "vpy").EscapePathStringForArg();

                result.Add(
                    ("node",
                        $"rmbox-vpygen.js {inputPath} {subtitlePath} {vpyPath}",
                        "null"));

                inputPath = vpyPath;
            }

            #endregion

            #region 处理视频

            string encodeMode = hwEncQualitySection["encode_mode"]?.ToObject<string>() switch
            {
                "cqp" => $"--cqp {hwEncQualitySection["cqp_value"]?.ToObject<string>()}",
                "2pass" => $"--vbr {hwEncQualitySection["2pass_value"]?.ToObject<int>()} --multipass 2pass-full",
                // ReSharper disable once NotResolvedInText
                _ => throw new ArgumentOutOfRangeException("encode_mode")
            };

            result.Add(
                (hwEncCore,
                    $"{(useVpy ? "--vpy" : "--avhw")} -i {inputPath} -o {outputPath} {(isVsfm ? "--audio-source " + atempPath : "")} {audioArgs} --codec {hwEncCodecSection["codec"]?.ToObject<string>()} {encodeMode} {(useCustomArgs ? customArgs : DefaultArgs)}",
                    hwEncCore));

            #endregion

            #region 清理临时文件

            result.AddRange(new[]
                {
                    vtempPath, atempPath, lwiPath
                }
                .Select(CommandExtension.GenerateTryDeleteCommand)
                .ToList());

            if (isVsfm)
                result.Add(CommandExtension.GenerateTryDeleteCommand(inputPath));

            #endregion

            return result;
        }

        private const string DefaultArgs = "";

        public Dictionary<string, JToken> RequiredConfigSections => new()
        {
            {
                ConfigSectionBase.IOConfigSectionId,
                JToken.FromObject(new
                {
                    support_subtitle = true,
                    support_vsfm = true,
                    output_suffix = "_encoded",
                    output_extension = ".mp4"
                })
            },
            {"Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.HwEncCoreConfigSection", new JObject()},
            {"Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.HwEncCodecConfigSection", new JObject()},
            {"Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.HwEncQualityConfigSection", new JObject()},
            {"Ruminoid.Toolbox.Plugins.Audio.ConfigSections.AudioConfigSection", new JObject()},
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
