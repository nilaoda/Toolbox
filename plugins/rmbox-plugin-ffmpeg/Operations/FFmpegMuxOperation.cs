using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Utils.Extensions;

namespace Ruminoid.Toolbox.Plugins.FFmpeg.Operations
{
    [Operation(
        "Ruminoid.Toolbox.Plugins.FFmpeg.Operations.FFmpegMuxOperation",
        "封装",
        "使用 FFmpeg 混流将音视频封装到一个媒体文件。")]
    public class FFmpegMuxOperation : IOperation
    {
        public List<(string Target, string Args, string Formatter)> Generate(Dictionary<string, JToken> sectionData)
        {
            JToken muxSection =
                sectionData["Ruminoid.Toolbox.Plugins.Common.ConfigSections.MuxConfigSection"];

            string videoPath = PathExtension.GetFullPathOrEmpty(muxSection["video"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();
            string audioPath = PathExtension.GetFullPathOrEmpty(muxSection["audio"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();
            string outputPath = PathExtension.GetFullPathOrEmpty(muxSection["output"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();

            string subtitlePathIntl = PathExtension.GetFullPathOrEmpty(muxSection["subtitle"]?.ToObject<string>() ?? string.Empty);
            string subtitlePath = subtitlePathIntl.EscapePathStringForArg();

            List<(string Target, string Args, string Formatter)> result = new();

            result.Add((
                "ffmpeg",
                $"-i {videoPath} -i {audioPath} {(string.IsNullOrEmpty(subtitlePathIntl) ? "" : $"-i {subtitlePath}")} -c:v copy -c:a copy -c:s mov_text {outputPath}",
                "ffmpeg"));

            return result;
        }

        public Dictionary<string, JToken> RequiredConfigSections => new()
        {
            {"Ruminoid.Toolbox.Plugins.Common.ConfigSections.MuxConfigSection", new JObject()}
        };
    }
}
