using System.Collections.Generic;
using Newtonsoft.Json;
using ReactiveUI;

namespace Ruminoid.Toolbox.Plugins.X264.ConfigSections.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class X264DemuxerConfigSectionViewModel : ReactiveObject
    {
        public List<string> DemuxerList { get; } = new() {"auto", "ffms", "lavf", "avs", "raw", "y4m"};

        [JsonProperty("demuxer")]
        private string _demuxer = "auto";

        public string Demuxer
        {
            get => _demuxer;
            set => this.RaiseAndSetIfChanged(ref _demuxer, value);
        }
    }
}
