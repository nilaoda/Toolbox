using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Utils.Extensions;

namespace Ruminoid.Toolbox.Plugins.FFmpeg.Operations
{
    [Operation(
        "Ruminoid.Toolbox.Plugins.FFmpeg.Operations.FFmpegCopyOperation",
        "格式转换（不压制）",
        "使用 FFmpeg 进行视频格式的转换（封装）。")]
    public class FFmpegCopyOperation : IOperation
    {
        public List<(string Target, string Args, string Formatter)> Generate(Dictionary<string, JToken> sectionData)
        {
            JToken ioSection =
                sectionData[ConfigSectionBase.IOConfigSectionId];

            string inputPath = PathExtension.GetFullPathOrEmpty(ioSection["input"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();
            string outputPath = PathExtension.GetFullPathOrEmpty(ioSection["output"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();

            return new List<(string, string, string)>
            {
                new(
                    "ffmpeg",
                    $"-i {inputPath} -y -c copy {outputPath}",
                    "ffmpeg")
            };
        }

        public Dictionary<string, JToken> RequiredConfigSections => new()
        {
            {ConfigSectionBase.IOConfigSectionId, new JObject()}
        };
    }
}
