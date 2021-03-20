using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Utils.Extensions;

namespace Ruminoid.Toolbox.Plugins.Mp4Box.Operations
{
    [Operation(
        "Ruminoid.Toolbox.Plugins.Mp4Box.Operations.Mp4BoxEncodeOperation",
        "小丸压制",
        "使用小丸压制法进行视频压制。")]
    public class Mp4BoxEncodeOperation : IOperation
    {
        public List<(string Target, string Args, string Formatter)> Generate(Dictionary<string, JToken> sectionData)
        {
            JToken ioSection =
                sectionData[ConfigSectionBase.IOConfigSectionId];
            JToken x264QualitySection =
                sectionData["Ruminoid.Toolbox.Plugins.X264.ConfigSections.X264EncodeQualityConfigSection"];
            JToken x264CoreSection =
                sectionData["Ruminoid.Toolbox.Plugins.X264.ConfigSections.X264CoreConfigSection"];
            JToken customArgsSection =
                sectionData["Ruminoid.Toolbox.Plugins.Common.ConfigSections.CustomArgsConfigSection"];

            string x264Core = x264CoreSection["core"]?.ToObject<string>();

            string customArgs = customArgsSection["args"]?.ToObject<string>();
            bool useCustomArgs = !string.IsNullOrWhiteSpace(customArgs);

            string defaultArgs =
                @"--preset 8 -I 300 -r 4 -b 3 --me umh -i 1 --scenecut 60 -f 1:1 --qcomp 0.5 --psy-rd 0.3:0 --aq-mode 2 --aq-strength 0.8";

            string inputPathIntl = PathExtension.GetFullPathOrEmpty(ioSection["input"]?.ToObject<string>() ?? string.Empty);
            string inputPath = inputPathIntl.EscapePathStringForArg();
            string subtitlePathIntl = PathExtension.GetFullPathOrEmpty(ioSection["subtitle"]?.ToObject<string>() ?? string.Empty);
            bool isIncludingSubtitle = !string.IsNullOrWhiteSpace(subtitlePathIntl);
            string subtitlePath = subtitlePathIntl.EscapePathStringForArg();
            string outputPath = PathExtension.GetFullPathOrEmpty(ioSection["output"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();
            string atempPath = Path.ChangeExtension(inputPathIntl, "atemp.aac").EscapePathStringForArg();
            string vtempPath = Path.ChangeExtension(inputPathIntl, "vtemp.mp4").EscapePathStringForArg();
            string vtempStatsPath = Path.ChangeExtension(inputPathIntl, "vtemp.stats").EscapePathStringForArg();
            string vtempStatsMbtreePath = Path.ChangeExtension(inputPathIntl, "vtemp.stats.mbtree").EscapePathStringForArg();

            List<(string, string, string)> result = new()
            {
                // Extract Audio
                new(
                    "ffmpeg",
                    $"-i {inputPath} -vn -sn -c:a copy -y -map 0:a:0 {atempPath}",
                    "ffmpeg")
            };

            switch (x264QualitySection["encode_mode"]?.ToObject<string>())
            {
                case "crf":
                    result.AddRange(new (string, string, string)[]
                    {
                        new(
                            x264Core,
                            $"--crf {x264QualitySection["crf_value"]?.ToObject<double>():N1} {(useCustomArgs ? customArgs : defaultArgs)} {(isIncludingSubtitle ? "--vf subtitles --sub " + subtitlePath : "")} -o {vtempPath} {inputPath}",
                            x264Core),
                        new(
                            "ffmpeg",
                            $"-i {vtempPath} -i {atempPath} -vcodec copy -acodec copy {outputPath}",
                            "ffmpeg"),
                        new(
                            "pwsh",
                            $"-Command Remove-Item {atempPath},{vtempPath}",
                            "null")
                    });
                    break;
                case "2pass":
                    result.AddRange(new (string, string, string)[]
                    {
                        new(
                            x264Core,
                            $"--pass 1 --bitrate {x264QualitySection["2pass_value"]?.ToObject<int>()} --stats {vtempStatsPath} {(useCustomArgs ? customArgs : defaultArgs)} {(isIncludingSubtitle ? "--vf subtitles --sub " + subtitlePath : "")} -o NUL {inputPath}",
                            x264Core),
                        new(
                            x264Core,
                            $"--pass 2 --bitrate {x264QualitySection["2pass_value"]?.ToObject<int>()} --stats {vtempStatsPath} {(useCustomArgs ? customArgs : defaultArgs)} {(isIncludingSubtitle ? "--vf subtitles --sub " + subtitlePath : "")} -o {vtempPath} {inputPath}",
                            x264Core),
                        new(
                            "ffmpeg",
                            $"-i {vtempPath} -i {atempPath} -vcodec copy -acodec copy {outputPath}",
                            "ffmpeg"),
                        new(
                            "pwsh",
                            $"-Command Remove-Item {atempPath},{vtempPath},{vtempStatsPath},{vtempStatsMbtreePath}",
                            "null")
                    });
                    break;
                default:
                    // ReSharper disable once NotResolvedInText
                    throw new ArgumentOutOfRangeException("encode_mode");
            }

            return result;
        }

        public Dictionary<string, JToken> RequiredConfigSections => new()
        {
            {
                ConfigSectionBase.IOConfigSectionId, JToken.FromObject(new
                {
                    support_subtitle = true
                })
            },
            {"Ruminoid.Toolbox.Plugins.X264.ConfigSections.X264CoreConfigSection", new JObject()},
            {"Ruminoid.Toolbox.Plugins.X264.ConfigSections.X264EncodeQualityConfigSection", new JObject()},
            {"Ruminoid.Toolbox.Plugins.Common.ConfigSections.CustomArgsConfigSection", new JObject()}
        };
    }
}
