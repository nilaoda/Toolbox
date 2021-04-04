using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Utils.Extensions;

namespace Ruminoid.Toolbox.Plugins.Mp4Box.Operations
{
    [Operation(
        "Ruminoid.Toolbox.Plugins.Mp4Box.Operations.Mp4BoxEncodeOperation",
        "小丸（CPU）压制",
        "使用小丸压制法进行视频压制。")]
    public class Mp4BoxEncodeOperation : IOperation
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
            bool isVsfm = ioSection["use_vsfm"].ToObject<bool>();

            bool useVpy = isVpy || isVsfm;

            if (isVpy && isVsfm)
                throw new OperationException(
                    "不支持在 VapourSynth 输入上使用 VSFilterMod。请在 VapourSynth 中完成字幕处理。",
                    typeof(Mp4BoxEncodeOperation));

            #endregion

            #region x264 分离器

            JToken x264DemuxerSection =
                sectionData["Ruminoid.Toolbox.Plugins.X264.ConfigSections.X264DemuxerConfigSection"];

            string demuxer = x264DemuxerSection["demuxer"]?.ToObject<string>();

            string demuxerArgs = string.IsNullOrWhiteSpace(demuxer) || demuxer == "auto" ? "" : "--demuxer " + demuxer;

            #endregion

            #region x264 质量

            JToken x264QualitySection =
                sectionData["Ruminoid.Toolbox.Plugins.X264.ConfigSections.X264EncodeQualityConfigSection"];

            #endregion

            #region x264 核心

            JToken x264CoreSection =
                sectionData["Ruminoid.Toolbox.Plugins.X264.ConfigSections.X264CoreConfigSection"];

            #endregion

            #region x264 相关

            string vtempPath = Path.ChangeExtension(inputPathIntl, "vtemp.mp4").EscapePathStringForArg();
            string vtempStatsPath = Path.ChangeExtension(inputPathIntl, "vtemp.stats").EscapePathStringForArg();
            string vtempStatsMbtreePath = Path.ChangeExtension(inputPathIntl, "vtemp.stats.mbtree").EscapePathStringForArg();

            string x264EncodeMode = x264QualitySection["encode_mode"]?.ToObject<string>() ?? string.Empty;

            string x264Core = x264CoreSection["core"]?.ToObject<string>();

            #endregion

            #region 音频

            JToken audioSection =
                sectionData["Ruminoid.Toolbox.Plugins.Audio.ConfigSections.AudioConfigSection"];

            #endregion

            #region 音频相关

            string atempPath = Path.ChangeExtension(inputPathIntl, "atemp.aac").EscapePathStringForArg();

            string audioMode = audioSection["mode"]?.ToObject<string>();
            bool hasAudio = audioMode != "none";
            int audioBitrate = audioSection["bitrate"].ToObject<int>();

            #endregion

            #region 输出相关

            string outputPath = PathExtension.GetFullPathOrEmpty(ioSection["output"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();

            #endregion

            #region 自定义参数

            JToken customArgsSection =
                sectionData["Ruminoid.Toolbox.Plugins.Common.ConfigSections.CustomArgsConfigSection"];

            string customArgs = customArgsSection["args"]?.ToObject<string>();
            bool useCustomArgs = !string.IsNullOrWhiteSpace(customArgs);

            #endregion

            #region 准备命令

            List<TaskCommand> result = new();

            #endregion

            // 开始处理

            #region 处理音频

            switch (audioMode)
            {
                case "copy":
                    result.Add(new(
                        "ffmpeg",
                        $"-i {inputPath} -vn -sn -c:a copy -y -map 0:a:0 {atempPath}",
                        "null"));
                    break;
                case "process":
                    result.Add(new(
                        "ffmpeg",
                        $"-i {inputPath} -vn -sn -v 0 -c:a pcm_s16le -f wav pipe: | {PathExtension.GetTargetPath("qaac64")} -q 2 --ignorelength -c {audioBitrate} - -o {atempPath}",
                        "null"));
                    break;
            }

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

            #region 处理 x264 参数

            string x264Args = $"{(useCustomArgs ? customArgs : DefaultArgs)} {(isIncludingSubtitle && !isVsfm ? "--vf subtitles --sub " + subtitlePath : "")}";

            #endregion

            #region 处理视频

            switch (x264EncodeMode)
            {
                case "crf":
                    result.Add(
                        GenerateVideoProcessingCommand(
                            $"--crf {x264QualitySection["crf_value"]?.ToObject<double>():N1} {demuxerArgs} {x264Args} -o {(hasAudio ? vtempPath : outputPath)}",
                            x264Core,
                            inputPath,
                            useVpy));
                    break;
                case "2pass":
                    result.AddRange(new[]
                    {
                        GenerateVideoProcessingCommand(
                            $"--pass 1 --bitrate {x264QualitySection["2pass_value"]?.ToObject<int>()} {demuxerArgs} --stats {vtempStatsPath} {x264Args} -o NUL",
                            x264Core,
                            inputPath,
                            useVpy),
                        GenerateVideoProcessingCommand(
                            $"--pass 2 --bitrate {x264QualitySection["2pass_value"]?.ToObject<int>()} {demuxerArgs} --stats {vtempStatsPath} {x264Args} -o {(hasAudio ? vtempPath : outputPath)}",
                            x264Core,
                            inputPath,
                            useVpy)
                    });
                    break;
                default:
                    // ReSharper disable once NotResolvedInText
                    throw new ArgumentOutOfRangeException("encode_mode");
            }

            #endregion

            #region 混流

            if (hasAudio)
                result.Add(new(
                    "ffmpeg",
                    $"-i {vtempPath} -i {atempPath} -vcodec copy -acodec copy -y {outputPath}",
                    "ffmpeg"));

            #endregion

            #region 清理临时文件

            result.AddRange(new[]
                {
                    vtempPath, atempPath, vtempStatsPath, vtempStatsMbtreePath, lwiPath
                }
                .Select(CommandExtension.GenerateTryDeleteCommand)
                .ToList());

            if (isVsfm)
                result.Add(CommandExtension.GenerateTryDeleteCommand(inputPath));

            #endregion

            return result;
        }

        public static TaskCommand GenerateVideoProcessingCommand(
            string args,
            string x264Core,
            string inputPath,
            bool useVpy) =>
            new(
                useVpy ? "VSPipe" : x264Core,
                (useVpy ? $"--y4m {inputPath} - | {PathExtension.GetTargetPath(x264Core)} --demuxer y4m - " : "") + args + (useVpy ? "" : $" {inputPath}"),
                x264Core);

        private const string DefaultArgs =
            @"--preset 8 -I 300 -r 4 -b 3 --me umh -i 1 --scenecut 60 -f 1:1 --qcomp 0.5 --psy-rd 0.3:0 --aq-mode 2 --aq-strength 0.8";

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
            {"Ruminoid.Toolbox.Plugins.X264.ConfigSections.X264CoreConfigSection", new JObject()},
            {"Ruminoid.Toolbox.Plugins.X264.ConfigSections.X264DemuxerConfigSection", new JObject()},
            {"Ruminoid.Toolbox.Plugins.X264.ConfigSections.X264EncodeQualityConfigSection", new JObject()},
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
