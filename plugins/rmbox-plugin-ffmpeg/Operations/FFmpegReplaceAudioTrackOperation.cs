using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Utils.Extensions;

namespace Ruminoid.Toolbox.Plugins.FFmpeg.Operations
{
    [Operation(
        "Ruminoid.Toolbox.Plugins.FFmpeg.Operations.FFmpegReplaceAudioTrackOperation",
        "替换音频",
        "保留视频，但将音频替换成另外输入的文件。")]
    public class FFmpegReplaceAudioTrackOperation : IOperation
    {
        public List<TaskCommand> Generate(Dictionary<string, JToken> sectionData)
        {
            JToken muxSection =
                sectionData["Ruminoid.Toolbox.Plugins.Common.ConfigSections.MuxConfigSection"];

            string videoPath = PathExtension.GetFullPathOrEmpty(muxSection["video"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();
            string audioPath = PathExtension.GetFullPathOrEmpty(muxSection["audio"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();
            string outputPath = PathExtension.GetFullPathOrEmpty(muxSection["output"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();

            #region 自定义参数

            JToken customArgsSection =
                sectionData["Ruminoid.Toolbox.Plugins.Common.ConfigSections.CustomArgsConfigSection"];

            string customArgs = customArgsSection["args"]?.ToObject<string>();
            bool useCustomArgs = !string.IsNullOrWhiteSpace(customArgs);

            #endregion

            return new List<TaskCommand>
            {
                new(
                    "ffmpeg",
                    $"-y -i {videoPath} -i {audioPath} {(useCustomArgs ? customArgs : DefaultArgs)} {outputPath}",
                    "ffmpeg")
            };
        }

        private const string DefaultArgs = "-map 0:v -map 1:a -c:v copy";

        public Dictionary<string, JToken> RequiredConfigSections => new()
        {
            {
                "Ruminoid.Toolbox.Plugins.Common.ConfigSections.MuxConfigSection",
                JObject.FromObject(new
                {
                    output_suffix = "_replaced"
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
