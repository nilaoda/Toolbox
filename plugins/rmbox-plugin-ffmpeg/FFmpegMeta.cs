using Ruminoid.Toolbox.Core;

namespace Ruminoid.Toolbox.Plugins.FFmpeg
{
    [RmboxPlugin]
    public class FFmpegMeta : IMeta
    {
        public string Name => "FFmpeg";
        public string Description => "提供了 FFmpeg 的基础功能和格式器。";
    }
}
