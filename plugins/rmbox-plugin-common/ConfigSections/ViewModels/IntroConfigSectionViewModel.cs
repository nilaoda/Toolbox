using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReactiveUI;

namespace Ruminoid.Toolbox.Plugins.Common.ConfigSections.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class IntroConfigSectionViewModel : ReactiveObject
    {
        #region Constructor

        public IntroConfigSectionViewModel(
            JToken sectionConfig)
        {
            MdText = sectionConfig["intro_text"]?.ToObject<string>() ?? "# 介绍获取失败\n\n插件似乎出现了问题。\n";
        }

        #endregion

        #region Data

        public string MdText { get; }

        #endregion
    }
}
