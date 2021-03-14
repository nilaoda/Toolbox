using System.Text;
using Ruminoid.Toolbox.Formatting;
using Ruminoid.Toolbox.Utils.Extensions;

namespace Ruminoid.Toolbox.Plugins.HwEnc.Formatters
{
    [Formatter("nvencc*|qsvencc*|vceencc*")]
    public class HwEncFormatter : IFormatter
    {
        public FormattedEvent Format(string target, string data)
        {
            bool parseProgress = double.TryParse(data.GetMidString("[", "%] "), out double progress);
            string frames = data.GetMidString("%] ", " frames: ");
            //string fps = data.GetMidString(" frames, ", " fps, ");
            string speed = data.GetMidString(" fps, ", " kb/s, remain ");
            string eta = data.GetMidString(" kb/s, remain ", ", GPU");
            string estSize = data.GetMidString(", est out size ", "MB");

            StringBuilder summaryBuilder = new();
            StringBuilder detailBuilder = new();

            if (parseProgress)
            {
                summaryBuilder.Append(progress.ToString("F1"));
                summaryBuilder.Append("% - ");
            }

            summaryBuilder.Append($"正在使用 {target} 压制 - ");
            summaryBuilder.Append(speed);
            summaryBuilder.Append(" kb/s");

            if (!string.IsNullOrWhiteSpace(frames))
            {
                detailBuilder.Append("编码 ");
                detailBuilder.Append(frames);
                detailBuilder.Append(" 帧");
            }
            
            if (!string.IsNullOrWhiteSpace(eta))
            {
                detailBuilder.Append("，剩余 ");
                detailBuilder.Append(eta);
            }

            return new(
                target,
                progress,
                !parseProgress,
                summaryBuilder.ToString(),
                detailBuilder.ToString());
        }
    }
}
