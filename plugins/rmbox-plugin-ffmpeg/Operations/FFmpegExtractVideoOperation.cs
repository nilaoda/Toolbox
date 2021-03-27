using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Utils.Extensions;

namespace Ruminoid.Toolbox.Plugins.FFmpeg.Operations
{
    [Operation(
        "Ruminoid.Toolbox.Plugins.FFmpeg.Operations.FFmpegExtractVideoOperation",
        "抽取视频",
        "使用 FFmpeg 抽取视频。")]
    public class FFmpegExtractVideoOperation : IOperation
    {
        public List<(string Target, string Args, string Formatter)> Generate(Dictionary<string, JToken> sectionData)
        {
            JToken ioSection =
                sectionData[ConfigSectionBase.IOConfigSectionId];

            string inputPath = PathExtension.GetFullPathOrEmpty(ioSection["input"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();
            string outputPath = PathExtension.GetFullPathOrEmpty(ioSection["output"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();

            #region 自定义参数

            JToken customArgsSection =
                sectionData["Ruminoid.Toolbox.Plugins.Common.ConfigSections.CustomArgsConfigSection"];

            string customArgs = customArgsSection["args"]?.ToObject<string>();
            bool useCustomArgs = !string.IsNullOrWhiteSpace(customArgs);

            string defaultArgs = "-c:v copy -an -sn -map 0:v:0";

            #endregion

            return new List<(string, string, string)>
            {
                new(
                    "ffmpeg",
                    $"-i {inputPath} -y {(useCustomArgs ? customArgs : defaultArgs)} {outputPath}",
                    "ffmpeg")
            };
        }

        public Dictionary<string, JToken> RequiredConfigSections => new()
        {
            {ConfigSectionBase.IOConfigSectionId, new JObject()},
            {"Ruminoid.Toolbox.Plugins.Common.ConfigSections.CustomArgsConfigSection", new JObject()}
        };
    }
}
