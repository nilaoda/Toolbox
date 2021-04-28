using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Ruminoid.Common2.Utils.UserTypes;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Utils.Extensions;

namespace Ruminoid.Toolbox.Plugins.FFmpeg.Operations
{
    [Operation(
        "Ruminoid.Toolbox.Plugins.FFmpeg.Operations.FFmpegStillImageOperation",
        "图片转视频（静止画视频）",
        "创建定长的静止图片视频。",
        RateValue.ThreeStars,
        "视频创建")]
    public class FFmpegStillImageOperation : IOperation
    {
        public List<TaskCommand> Generate(Dictionary<string, JToken> sectionData)
        {
            #region IO 参数

            JToken ioSection =
                sectionData[ConfigSectionBase.IOConfigSectionId];

            string inputPath = PathExtension.GetFullPathOrEmpty(ioSection["input"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();
            string outputPath = PathExtension.GetFullPathOrEmpty(ioSection["output"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();

            #endregion

            #region 一图流参数

            JToken pictureFlowSection =
                sectionData["Ruminoid.Toolbox.Plugins.Common.ConfigSections.PictureFlowConfigSection"];

            // ReSharper disable PossibleNullReferenceException

            int frameRate = pictureFlowSection["frame_rate"].ToObject<int>();
            double crfValue = pictureFlowSection["crf_value"].ToObject<double>();
            int duration = pictureFlowSection["duration"].ToObject<int>();

            // ReSharper restore PossibleNullReferenceException

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
                    $"-y -loop 1 -r {frameRate} -i {inputPath} -c:v libx264 -crf {crfValue} -t {duration} {(useCustomArgs ? customArgs : DefaultArgs)} {outputPath}",
                    "ffmpeg")
            };
        }

        private const string DefaultArgs = "-tune stillimage -pix_fmt yuv420p";

        public Dictionary<string, JToken> RequiredConfigSections => new()
        {
            {
                ConfigSectionBase.IOConfigSectionId,
                JToken.FromObject(new
                {
                    output_suffix = "_video",
                    output_extension = ".mp4"
                })
            },
            {"Ruminoid.Toolbox.Plugins.Common.ConfigSections.PictureFlowConfigSection", new JObject()},
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
