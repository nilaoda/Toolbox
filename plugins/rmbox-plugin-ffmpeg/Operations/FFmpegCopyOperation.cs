using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;

namespace Ruminoid.Toolbox.Plugins.FFmpeg.Operations
{
    [Operation(
        "Ruminoid.Toolbox.Plugins.FFmpeg.Operations.FFmpegEncodeOperation",
        "格式转换（不压制）",
        "使用 FFmpeg 进行视频格式的转换（封装）。")]
    public class FFmpegCopyOperation : IOperation
    {
        public List<Tuple<string, string>> Generate(Dictionary<string, JToken> sectionData)
        {
            JToken ioSection = sectionData["Ruminoid.Toolbox.Plugins.Common.ConfigSections.IOConfigSection"];

            return new List<Tuple<string, string>>
            {
                new(
                    "ffmpeg",
                    $"-i {Path.GetFullPath(ioSection["video"]?.ToObject<string>() ?? string.Empty)} -c copy {Path.GetFullPath(ioSection["output"]?.ToObject<string>() ?? string.Empty)}")
            };
        }

        public List<string> RequiredConfigSections => new()
        {
            "Ruminoid.Toolbox.Plugins.Common.ConfigSections.IOConfigSection"
        };
    }
}
