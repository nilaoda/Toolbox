using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Ruminoid.Common2.Utils.UserTypes;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Utils.Extensions;

namespace Ruminoid.Toolbox.Plugins.FFmpeg.Operations
{
    [Operation(
        "Ruminoid.Toolbox.Plugins.FFmpeg.Operations.FFmpegCopyOperation",
        "格式转换（不压制）",
        "使用 FFmpeg 进行视频格式的转换（封装）。",
        RateValue.ThreeStars,
        "封装转换")]
    public class FFmpegCopyOperation : IOperation
    {
        public List<TaskCommand> Generate(Dictionary<string, JToken> sectionData)
        {
            JToken ioSection =
                sectionData[ConfigSectionBase.IOConfigSectionId];

            string inputPath = PathExtension.GetFullPathOrEmpty(ioSection["input"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();
            string subtitlePathIntl = PathExtension.GetFullPathOrEmpty(ioSection["subtitle"]?.ToObject<string>() ?? string.Empty);
            string subtitlePath = subtitlePathIntl.EscapePathStringForArg();
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
                    $"-i {inputPath} {(string.IsNullOrEmpty(subtitlePathIntl) ? "" : $"-i {subtitlePath}")} -y {(useCustomArgs ? customArgs : DefaultArgs)} {outputPath}",
                    "ffmpeg")
            };
        }

        private const string DefaultArgs = "-c:v copy -c:a copy -c:s mov_text";

        public Dictionary<string, JToken> RequiredConfigSections => new()
        {
            {
                ConfigSectionBase.IntroConfigSectionId,
                JObject.FromObject(new
                {
                    intro_text =
                        typeof(FFmpegCopyOperation).Assembly
                            .GetManifestResourceStream(
                                "Ruminoid.Toolbox.Plugins.FFmpeg.Resources.Markdowns.FFmpegCopyOperation.md")
                            .ReadStreamToEnd()
                })
            },
            {
                ConfigSectionBase.IOConfigSectionId,
                JObject.FromObject(new
                {
                    support_subtitle = true
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
