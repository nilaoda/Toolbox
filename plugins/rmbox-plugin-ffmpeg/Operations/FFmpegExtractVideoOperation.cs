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
        public List<TaskCommand> Generate(Dictionary<string, JToken> sectionData)
        {
            JToken ioSection =
                sectionData[ConfigSectionBase.IOConfigSectionId];

            string inputPath = PathExtension.GetFullPathOrEmpty(ioSection["input"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();
            string outputPath = PathExtension.GetFullPathOrEmpty(ioSection["output"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();

            #region 自定义参数

            JToken customArgsSection =
                sectionData["Ruminoid.Toolbox.Plugins.Common.ConfigSections.CustomArgsConfigSection"];

            string customArgs = customArgsSection["args"]?.ToObject<string>();
            bool useCustomArgs = customArgsSection["use_custom_args"]?.ToObject<bool>() ?? false;

            #endregion

            return new List<TaskCommand>
            {
                new(
                    "ffmpeg",
                    $"-i {inputPath} -y {(useCustomArgs ? customArgs : DefaultArgs)} {outputPath}",
                    "ffmpeg")
            };
        }

        private const string DefaultArgs = "-c:v copy -an -sn -map 0:v:0";

        public Dictionary<string, JToken> RequiredConfigSections => new()
        {
            {
                ConfigSectionBase.IOConfigSectionId,
                JToken.FromObject(new
                {
                    output_suffix = "_video"
                })
            },
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
