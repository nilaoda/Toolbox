using Ruminoid.Toolbox.Core;

namespace Ruminoid.Toolbox.Plugins.HwEnc
{
    [RmboxPlugin]
    public class HwEncMeta : IMeta
    {
        public string Name => "显卡压制";
        public string Description => "提供了显卡压制（NVEnc/QSVEnc/VCEEnc）的基础功能、操作和格式器。";
    }
}
