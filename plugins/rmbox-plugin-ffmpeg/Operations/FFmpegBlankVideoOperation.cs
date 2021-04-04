using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Utils.Extensions;

namespace Ruminoid.Toolbox.Plugins.FFmpeg.Operations
{
    [Operation(
        "Ruminoid.Toolbox.Plugins.FFmpeg.Operations.FFmpegBlankVideoOperation",
        "创建空白视频",
        "创建一段指定大小和时长的空白视频。")]
    public class FFmpegBlankVideoOperation : IOperation
    {
        public List<TaskCommand> Generate(Dictionary<string, JToken> sectionData)
        {
            JToken ioSection =
                sectionData["Ruminoid.Toolbox.Plugins.Common.ConfigSections.OutputConfigSection"];

            string outputPath = PathExtension.GetFullPathOrEmpty(ioSection["output"]?.ToObject<string>() ?? string.Empty).EscapePathStringForArg();

            #region 一图流参数

            JToken pictureFlowSection =
                sectionData["Ruminoid.Toolbox.Plugins.Common.ConfigSections.PictureFlowConfigSection"];

            // ReSharper disable PossibleNullReferenceException

            int frameRate = pictureFlowSection["frame_rate"].ToObject<int>();
            double crfValue = pictureFlowSection["crf_value"].ToObject<double>();
            string size = pictureFlowSection["size"].ToObject<string>();
            int duration = pictureFlowSection["duration"].ToObject<int>();

            // ReSharper restore PossibleNullReferenceException

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
                    $"-y -f lavfi -i color=size={size}:rate={frameRate}:color=black -f lavfi -i anullsrc=channel_layout=stereo:sample_rate=44100 -c:v libx264 -crf {crfValue} -t {duration} {(useCustomArgs ? customArgs : DefaultArgs)} {outputPath}",
                    "ffmpeg")
            };
        }

        private const string DefaultArgs = "";

        public Dictionary<string, JToken> RequiredConfigSections => new()
        {
            {"Ruminoid.Toolbox.Plugins.Common.ConfigSections.OutputConfigSection", new JObject()},
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
