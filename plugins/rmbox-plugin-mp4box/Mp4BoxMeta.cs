using Ruminoid.Toolbox.Core;

namespace Ruminoid.Toolbox.Plugins.Mp4Box
{
    [RmboxPlugin]
    public class Mp4BoxMeta : IMeta
    {
        public string Name => "Mp4Box";
        public string Description => "提供了 CPU 压制等功能。";
        public string Author => "Il Harper";
    }
}
