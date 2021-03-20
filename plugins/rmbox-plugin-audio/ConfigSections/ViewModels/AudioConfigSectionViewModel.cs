using Newtonsoft.Json;
using ReactiveUI;

namespace Ruminoid.Toolbox.Plugins.Audio.ConfigSections.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AudioConfigSectionViewModel : ReactiveObject
    {
        #region Data

        [JsonProperty("mode")]
        private string _encodeMode = "copy";

        public string EncodeMode
        {
            get => _encodeMode;
            set => this.RaiseAndSetIfChanged(ref _encodeMode, value);
        }

        [JsonProperty("bitrate")]
        private int _bitrate = 320;

        public int Bitrate
        {
            get => _bitrate;
            set => this.RaiseAndSetIfChanged(ref _bitrate, value);
        }

        #endregion
    }
}
