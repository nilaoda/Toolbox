using Ruminoid.Toolbox.Core;

namespace Ruminoid.Toolbox.Plugins.X264
{
    [RmboxPlugin]
    public class X264Meta : IMeta
    {
        public string Name => "X264";
        public string Description => "提供了 X264 的基础功能和格式器。";
        public string Author => "Il Harper";
    }
}
