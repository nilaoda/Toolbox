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
        public List<TaskCommand> Generate(Dictionary<string, JToken> sectionData)
        {
            JToken muxSection =
                sectionData["Ruminoid.Toolbox.Plugins.Common.ConfigSections.MuxConfigSection"];

            string videoPath = PathExtension.GetFullPathOrEmpty(muxSection["video"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();
            string audioPath = PathExtension.GetFullPathOrEmpty(muxSection["audio"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();
            string outputPath = PathExtension.GetFullPathOrEmpty(muxSection["output"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();

            string subtitlePathIntl = PathExtension.GetFullPathOrEmpty(muxSection["subtitle"]?.ToObject<string>() ?? string.Empty);
            string subtitlePath = subtitlePathIntl.EscapePathStringForArg();

            #region 混流选项

            JToken muxOptionsSection =
                sectionData["Ruminoid.Toolbox.Plugins.Common.ConfigSections.MuxOptionsConfigSection"];

            bool useShortest = muxOptionsSection["use_shortest"].ToObject<bool>();

            #endregion

            #region 自定义参数

            JToken customArgsSection =
                sectionData["Ruminoid.Toolbox.Plugins.Common.ConfigSections.CustomArgsConfigSection"];

            string customArgs = customArgsSection["args"]?.ToObject<string>();
            bool useCustomArgs = customArgsSection["use_custom_args"]?.ToObject<bool>() ?? false;

            #endregion

            List<TaskCommand> result = new();

            result.Add((
                "ffmpeg",
                $"-i {videoPath} -i {audioPath} {(string.IsNullOrEmpty(subtitlePathIntl) ? "" : $"-i {subtitlePath}")} -y {(useCustomArgs ? customArgs : DefaultArgs)} {(useShortest ? "-shortest" : "")} {outputPath}",
                "ffmpeg"));

            return result;
        }

        private const string DefaultArgs = "-c:v copy -c:a copy -c:s mov_text";

        public Dictionary<string, JToken> RequiredConfigSections => new()
        {
            {
                "Ruminoid.Toolbox.Plugins.Common.ConfigSections.MuxConfigSection",
                JObject.FromObject(new
                {
                    output_suffix = "_muxed"
                })
            },
            {"Ruminoid.Toolbox.Plugins.Common.ConfigSections.MuxOptionsConfigSection", new JObject()},
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
