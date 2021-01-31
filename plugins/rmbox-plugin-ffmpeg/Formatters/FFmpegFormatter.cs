using Ruminoid.Toolbox.Formatting;

namespace Ruminoid.Toolbox.Plugins.FFmpeg.Formatters
{
    [Formatter("ffmpeg")]
    public class FFmpegFormatter : IFormatter
    {
        public FormattedEvent Format(string target, string data)
        {
            return new(target, 0, "", "");
        }
    }
}
