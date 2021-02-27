using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Utils.Extensions;

namespace Ruminoid.Toolbox.Plugins.FFmpeg.Operations
{
    [Operation(
        "Ruminoid.Toolbox.Plugins.FFmpeg.Operations.FFmpegEncodeOperation",
        "FFmpeg 压制",
        "使用 FFmpeg 进行视频压制（重编码）。")]
    public class FFmpegEncodeOperation : IOperation
    {
        public List<Tuple<string, string>> Generate(Dictionary<string, JToken> sectionData)
        {
            JToken ioSection =
                sectionData["Ruminoid.Toolbox.Plugins.Common.ConfigSections.IOConfigSection"];

            string videoPath = Path.GetFullPath(ioSection["video"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();
            string outputPath = Path.GetFullPath(ioSection["output"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();

            return new List<Tuple<string, string>>
            {
                new(
                    "ffmpeg",
                    $"-i {videoPath} {outputPath}")
            };
        }

        public List<string> RequiredConfigSections => new()
        {
            "Ruminoid.Toolbox.Plugins.Common.ConfigSections.IOConfigSection"
        };
    }
}
