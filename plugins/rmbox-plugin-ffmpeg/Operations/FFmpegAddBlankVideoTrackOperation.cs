using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Utils.Extensions;

namespace Ruminoid.Toolbox.Plugins.FFmpeg.Operations
{
    [Operation(
        "Ruminoid.Toolbox.Plugins.FFmpeg.Operations.FFmpegAddBlankVideoTrackOperation",
        "音频转视频（黑屏一图流）",
        "为音频创建黑屏视频。")]
    public class FFmpegAddBlankVideoTrackOperation : IOperation
    {
        public List<TaskCommand> Generate(Dictionary<string, JToken> sectionData)
        {
            #region IO 参数

            JToken ioSection =
                sectionData[ConfigSectionBase.IOConfigSectionId];

            string inputPathIntl = PathExtension.GetFullPathOrEmpty(ioSection["input"]?.ToObject<string>() ?? string.Empty);
            string inputPath = inputPathIntl.EscapePathStringForArg();
            string outputPath = PathExtension.GetFullPathOrEmpty(ioSection["output"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();

            #endregion

            #region 一图流参数

            JToken pictureFlowSection =
                sectionData["Ruminoid.Toolbox.Plugins.Common.ConfigSections.PictureFlowConfigSection"];

            // ReSharper disable PossibleNullReferenceException

            int frameRate = pictureFlowSection["frame_rate"].ToObject<int>();
            double crfValue = pictureFlowSection["crf_value"].ToObject<double>();
            string size = pictureFlowSection["size"].ToObject<string>();

            // ReSharper restore PossibleNullReferenceException

            #endregion

            #region 流处理模式

            JToken advancedTrackSection =
                sectionData["Ruminoid.Toolbox.Plugins.Common.ConfigSections.AdvancedTrackConfigSection"];

            string processMode = advancedTrackSection["process_mode"]?.ToObject<string>() ?? string.Empty;

            string audioProcessMode = inputPathIntl switch
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
            bool useCustomArgs = customArgsSection["use_custom_args"]?.ToObject<bool>() ?? false;

            #endregion

            return new List<TaskCommand>
            {
                new(
                    "ffmpeg",
                    $"-y -f lavfi -i color=size={size}:rate={frameRate}:color=black -i {inputPath} -c:v libx264 -c:a {audioProcessMode} -crf {crfValue} -shortest {(useCustomArgs ? customArgs : DefaultArgs)} {outputPath}",
                    "ffmpeg")
            };
        }

        private const string DefaultArgs = "";

        public Dictionary<string, JToken> RequiredConfigSections => new()
        {
            {
                ConfigSectionBase.IOConfigSectionId,
                JToken.FromObject(new
                {
                    output_suffix = "_blankvideo",
                    output_extension = ".mp4"
                })
            },
            {"Ruminoid.Toolbox.Plugins.Common.ConfigSections.PictureFlowConfigSection", new JObject()},
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
