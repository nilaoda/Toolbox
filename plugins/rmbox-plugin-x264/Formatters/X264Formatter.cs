using Ruminoid.Toolbox.Formatting;
using Ruminoid.Toolbox.Utils.Extensions;

namespace Ruminoid.Toolbox.Plugins.X264.Formatters
{
    [Formatter("x264")]
    public class X264Formatter : IFormatter
    {
        public FormattedEvent Format(string target, string data)
        {
            bool parseProgress = double.TryParse(data.GetMidString("[", "%] "), out double progress);
            string frames = data.GetMidString("%] ", " frames, ");
            string fps = data.GetMidString(" frames, ", " fps, ");
            string speed = data.GetMidString(" fps, ", " kb/s, ");
            string size = data.GetMidString(" kb/s, ", " MB, eta ");
            string eta = data.GetMidString(" MB, eta ", ", est.size ");
            string estSize = data.GetMidString(", est.size ", "MB");

            return new(
                target,
                progress,
                !parseProgress,
                $"正在使用 X264 压制 - {speed} kb/s",
                $"编码 {frames} 帧，大小 {size}/{estSize} MB，剩余 {eta}");
        }
    }
}
