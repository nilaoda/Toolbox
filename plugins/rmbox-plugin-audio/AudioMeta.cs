using Ruminoid.Toolbox.Core;

namespace Ruminoid.Toolbox.Plugins.Audio
{
    [RmboxPlugin]
    public class AudioMeta : IMeta
    {
        public string Name => "音频压制";
        public string Description => "提供了音频压制相关的功能。";
    }
}
