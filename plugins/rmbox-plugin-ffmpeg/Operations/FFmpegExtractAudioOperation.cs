using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Utils.Extensions;

namespace Ruminoid.Toolbox.Plugins.FFmpeg.Operations
{
    [Operation(
        "Ruminoid.Toolbox.Plugins.FFmpeg.Operations.FFmpegExtractAudioOperation",
        "抽取音频",
        "使用 FFmpeg 抽取音频。")]
    public class FFmpegExtractAudioOperation : IOperation
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
                    $"-i {inputPath} -vn -sn -c:a copy -y -map 0:a:0 {outputPath}",
                    "ffmpeg")
            };
        }

        public Dictionary<string, JToken> RequiredConfigSections => new()
        {
            {ConfigSectionBase.IOConfigSectionId, new JObject()}
        };
    }
}
