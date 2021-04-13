using System;
using System.Collections.Generic;
using System.IO;
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
            string audioPathIntl = PathExtension.GetFullPathOrEmpty(muxSection["audio"]?.ToObject<string>() ?? string.Empty);
            string audioPath = audioPathIntl.EscapePathStringForArg();
            string outputPath = PathExtension.GetFullPathOrEmpty(muxSection["output"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();

            #region 流处理模式

            JToken advancedTrackSection =
                sectionData["Ruminoid.Toolbox.Plugins.Common.ConfigSections.AdvancedTrackConfigSection"];

            string processMode = advancedTrackSection["process_mode"]?.ToObject<string>() ?? string.Empty;

            string audioProcessMode = audioPathIntl switch
            {
                { } e when Path.GetExtension(e) == ".aac" => "copy",
                _ => processMode switch
                {
                    "force_copy" => "copy",
                    _ => "aac"
                }
            };

            #endregion

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
                    $"-y -i {videoPath} -i {audioPath} -map 0:v -map 1:a -c:v copy -c:a {audioProcessMode} {(useCustomArgs ? customArgs : DefaultArgs)} {outputPath}",
                    "ffmpeg")
            };
        }

        private const string DefaultArgs = "";

        public Dictionary<string, JToken> RequiredConfigSections => new()
        {
            {
                "Ruminoid.Toolbox.Plugins.Common.ConfigSections.MuxConfigSection",
                JObject.FromObject(new
                {
                    output_suffix = "_replaced"
                })
            },
            {"Ruminoid.Toolbox.Plugins.Common.ConfigSections.AdvancedTrackConfigSection", new JObject()},
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
