using Newtonsoft.Json;
using ReactiveUI;

namespace Ruminoid.Toolbox.Plugins.Common.ConfigSections
{
    [JsonObject(MemberSerialization.OptIn)]
    public class IOConfigSectionData : ReactiveObject
    {
        [JsonProperty("video")]
        private string _video = "";

        public string Video
        {
            get => _video;
            set => this.RaiseAndSetIfChanged(ref _video, value);
        }

        [JsonProperty("subtitle")]
        private string _subtitle = "";

        public string Subtitle
        {
            get => _subtitle;
            set => this.RaiseAndSetIfChanged(ref _subtitle, value);
        }

        [JsonProperty("output")]
        private string _output = "";

        public string Output
        {
            get => _output;
            set => this.RaiseAndSetIfChanged(ref _output, value);
        }
    }
}
