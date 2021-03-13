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
        public List<(string Target, string Args)> Generate(Dictionary<string, JToken> sectionData)
        {
            JToken ioSection =
                sectionData["Ruminoid.Toolbox.Plugins.Common.ConfigSections.IOConfigSection"];
            JToken x264QualitySection =
                sectionData["Ruminoid.Toolbox.Plugins.X264.ConfigSections.X264EncodeQualityConfigSection"];

            string videoPathIntl = Path.GetFullPath(ioSection["video"]?.ToObject<string>() ?? string.Empty);
            string videoPath = videoPathIntl.EscapePathStringForArg();
            string outputPath = Path.GetFullPath(ioSection["output"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();
            string atempPath = Path.ChangeExtension(videoPathIntl, "atemp.aac").EscapePathStringForArg();
            string vtempPath = Path.ChangeExtension(videoPathIntl, "vtemp.mp4").EscapePathStringForArg();
            string vtempStatsPath = Path.ChangeExtension(videoPathIntl, "vtemp.stats").EscapePathStringForArg();
            string vtempStatsMbtreePath = Path.ChangeExtension(videoPathIntl, "vtemp.stats.mbtree").EscapePathStringForArg();

            List<(string, string)> result = new()
            {
                // Extract Audio
                new(
                    "ffmpeg",
                    $"-i {videoPath} -vn -sn -c:a copy -y -map 0:a:0 {atempPath}")
            };

            switch (x264QualitySection["encode_mode"]?.ToObject<string>())
            {
                case "crf":
                    result.AddRange(new (string, string)[]
                    {
                        new(
                            "x264",
                            $"--crf {x264QualitySection["crf_value"]?.ToObject<double>():N1} --preset 8 -I 300 -r 4 -b 3 --me umh -i 1 --scenecut 60 -f 1:1 --qcomp 0.5 --psy-rd 0.3:0 --aq-mode 2 --aq-strength 0.8 -o {vtempPath} {videoPath}"),
                        new(
                            "ffmpeg",
                            $"-i {vtempPath} -i {atempPath} -vcodec copy -acodec copy {outputPath}"),
                        new(
                            "pwsh",
                            $"-Command Remove-Item {atempPath} {vtempPath}")
                    });
                    break;
                case "2pass":
                    result.AddRange(new (string, string)[]
                    {
                        new(
                            "x264",
                            $"--pass 1 --bitrate {x264QualitySection["2pass_value"]?.ToObject<int>()} --stats {vtempStatsPath} --preset 8  -I 300 -r 4 -b 3 --me umh -i 1 --scenecut 60 -f 1:1 --qcomp 0.5 --psy-rd 0.3:0 --aq-mode 2 --aq-strength 0.8 -o NUL {videoPath}"),
                        new(
                            "x264",
                            $"--pass 2 --bitrate {x264QualitySection["2pass_value"]?.ToObject<int>()} --stats {vtempStatsPath} --preset 8  -I 300 -r 4 -b 3 --me umh -i 1 --scenecut 60 -f 1:1 --qcomp 0.5 --psy-rd 0.3:0 --aq-mode 2 --aq-strength 0.8 -o {vtempPath} {videoPath}"),
                        new(
                            "ffmpeg",
                            $"-i {vtempPath} -i {atempPath} -vcodec copy -acodec copy {outputPath}"),
                        new(
                            "pwsh",
                            $"-Command Remove-Item {atempPath},{vtempPath},{vtempStatsPath},{vtempStatsMbtreePath}")
                    });
                    break;
                default:
                    // ReSharper disable once NotResolvedInText
                    throw new ArgumentOutOfRangeException("encode_mode");
            }

            return result;
        }

        public List<string> RequiredConfigSections => new()
        {
            "Ruminoid.Toolbox.Plugins.Common.ConfigSections.IOConfigSection",
            "Ruminoid.Toolbox.Plugins.X264.ConfigSections.X264EncodeQualityConfigSection"
        };
    }
}
