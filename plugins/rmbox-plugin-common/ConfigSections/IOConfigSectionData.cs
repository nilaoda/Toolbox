using Newtonsoft.Json;

namespace Ruminoid.Toolbox.Plugins.Common.ConfigSections
{
    [JsonObject(MemberSerialization.OptIn)]
    public class IOConfigSectionData
    {
        [JsonProperty("video")]
        public string Video { get; set; }

        [JsonProperty("subtitle")]
        public string Subtitle { get; set; }

        [JsonProperty("output")]
        public string Output { get; set; }
    }
}
