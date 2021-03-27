using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Utils.Extensions;

namespace Ruminoid.Toolbox.Plugins.FFmpeg.Operations
{
    [Operation(
        "Ruminoid.Toolbox.Plugins.FFmpeg.Operations.FFmpegMuxSubtitleOperation",
        "内封字幕",
        "使用 FFmpeg 封装字幕到视频。注意这不是压制。")]
    public class FFmpegMuxSubtitleOperation : IOperation
    {
        public List<(string Target, string Args, string Formatter)> Generate(Dictionary<string, JToken> sectionData)
        {
            JToken ioSection =
                sectionData[ConfigSectionBase.IOConfigSectionId];

            string inputPath = PathExtension.GetFullPathOrEmpty(ioSection["input"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();
            string subtitlePath = PathExtension.GetFullPathOrEmpty(ioSection["subtitle"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();
            string outputPath = PathExtension.GetFullPathOrEmpty(ioSection["output"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();

            List<(string Target, string Args, string Formatter)> result = new();

            result.Add((
                "ffmpeg",
                $"-i {inputPath} -i {subtitlePath} -y -c:v copy -c:a copy -c:s mov_text {outputPath}",
                "ffmpeg"));

            return result;
        }

        public Dictionary<string, JToken> RequiredConfigSections => new()
        {
            {
                ConfigSectionBase.IOConfigSectionId, JObject.FromObject(new
                {
                    support_subtitle = true
                })
            }
        };
    }
}
